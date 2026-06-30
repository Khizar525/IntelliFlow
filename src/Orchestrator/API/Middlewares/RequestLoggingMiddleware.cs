using System.Diagnostics;

namespace Orchestrator.API.Middlewares;

/// <summary>
/// Middleware for logging HTTP requests and responses.
/// Adds correlation IDs and measures request duration.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
            ?? Guid.NewGuid().ToString();
        
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Append("X-Correlation-ID", correlationId);

        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var method = context.Request.Method;

        _logger.LogInformation("Incoming request: {Method} {Path} | CorrelationId: {CorrelationId}", 
            method, requestPath, correlationId);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var duration = stopwatch.ElapsedMilliseconds;

            if (statusCode >= 400)
            {
                _logger.LogWarning("Request completed: {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | CorrelationId: {CorrelationId}", 
                    method, requestPath, statusCode, duration, correlationId);
            }
            else
            {
                _logger.LogInformation("Request completed: {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | CorrelationId: {CorrelationId}", 
                    method, requestPath, statusCode, duration, correlationId);
            }
        }
    }
}

/// <summary>
/// Extension method to add RequestLoggingMiddleware to the pipeline.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
