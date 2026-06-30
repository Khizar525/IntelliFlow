using System.Collections.Concurrent;

namespace Orchestrator.API.Middlewares;

/// <summary>
/// Middleware for rate limiting to prevent abuse.
/// Tracks request counts per IP address and blocks excessive requests.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly ConcurrentDictionary<string, ClientInfo> _clients = new();
    private readonly Timer _cleanupTimer;

    // Configuration
    private readonly int _maxRequestsPerMinute = 30;
    private readonly int _maxLoginAttemptsPerMinute = 5;
    private readonly TimeSpan _blockDuration = TimeSpan.FromMinutes(5);

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        // Cleanup timer to remove expired client entries
        _cleanupTimer = new Timer(CleanupExpiredClients, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // Check if client is blocked
        if (IsClientBlocked(clientIp))
        {
            _logger.LogWarning("Blocked request from {ClientIp} - temporarily blocked due to rate limiting", clientIp);
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Too many requests",
                message = "You have been temporarily blocked due to excessive requests. Please try again later.",
                retryAfter = _blockDuration.TotalSeconds
            });
            return;
        }

        // Track request
        var isLoginAttempt = path.Contains("/login");
        TrackRequest(clientIp, isLoginAttempt);

        // Check rate limits
        if (IsRateLimited(clientIp, isLoginAttempt))
        {
            _logger.LogWarning("Rate limit exceeded for {ClientIp}", clientIp);
            BlockClient(clientIp);
            
            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = "Too many requests. You have been temporarily blocked.",
                retryAfter = _blockDuration.TotalSeconds
            });
            return;
        }

        // Add rate limit headers
        AddRateLimitHeaders(context, clientIp);

        await _next(context);
    }

    private bool IsClientBlocked(string clientIp)
    {
        if (_clients.TryGetValue(clientIp, out var client))
        {
            return client.BlockedUntil.HasValue && client.BlockedUntil > DateTime.UtcNow;
        }
        return false;
    }

    private void TrackRequest(string clientIp, bool isLoginAttempt)
    {
        var client = _clients.GetOrAdd(clientIp, _ => new ClientInfo());

        lock (client)
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-1);

            // Clean old requests
            client.Requests.RemoveAll(r => r < windowStart);

            // Add current request
            client.Requests.Add(now);

            if (isLoginAttempt)
            {
                client.LoginAttempts.RemoveAll(r => r < windowStart);
                client.LoginAttempts.Add(now);
            }
        }
    }

    private bool IsRateLimited(string clientIp, bool isLoginAttempt)
    {
        if (!_clients.TryGetValue(clientIp, out var client))
            return false;

        lock (client)
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-1);

            // Clean old requests
            client.Requests.RemoveAll(r => r < windowStart);
            client.LoginAttempts.RemoveAll(r => r < windowStart);

            // Check limits
            if (client.Requests.Count > _maxRequestsPerMinute)
            {
                _logger.LogWarning("Request limit exceeded for {ClientIp}: {Count} requests in last minute", 
                    clientIp, client.Requests.Count);
                return true;
            }

            if (isLoginAttempt && client.LoginAttempts.Count > _maxLoginAttemptsPerMinute)
            {
                _logger.LogWarning("Login attempt limit exceeded for {ClientIp}: {Count} attempts in last minute", 
                    clientIp, client.LoginAttempts.Count);
                return true;
            }

            return false;
        }
    }

    private void BlockClient(string clientIp)
    {
        if (_clients.TryGetValue(clientIp, out var client))
        {
            lock (client)
            {
                client.BlockedUntil = DateTime.UtcNow.Add(_blockDuration);
                client.Requests.Clear();
                client.LoginAttempts.Clear();
            }
        }
    }

    private void AddRateLimitHeaders(HttpContext context, string clientIp)
    {
        if (_clients.TryGetValue(clientIp, out var client))
        {
            lock (client)
            {
                var remaining = _maxRequestsPerMinute - client.Requests.Count;
                context.Response.Headers.Append("X-RateLimit-Limit", _maxRequestsPerMinute.ToString());
                context.Response.Headers.Append("X-RateLimit-Remaining", Math.Max(0, remaining).ToString());
                context.Response.Headers.Append("X-RateLimit-Reset", 
                    DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString());
            }
        }
    }

    private void CleanupExpiredClients(object? state)
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _clients.Where(kvp => 
            !kvp.Value.BlockedUntil.HasValue && 
            kvp.Value.Requests.Count == 0 &&
            kvp.Value.LoginAttempts.Count == 0)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _clients.TryRemove(key, out _);
        }

        _logger.LogDebug("Cleaned up {Count} expired client entries", expiredKeys.Count);
    }
}

/// <summary>
/// Tracks client request information for rate limiting.
/// </summary>
public class ClientInfo
{
    public List<DateTime> Requests { get; } = new();
    public List<DateTime> LoginAttempts { get; } = new();
    public DateTime? BlockedUntil { get; set; }
}

/// <summary>
/// Extension method to add RateLimitingMiddleware to the pipeline.
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}
