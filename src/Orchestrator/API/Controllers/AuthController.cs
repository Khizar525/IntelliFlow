// ============================================================
// Module 1: Auth Controller — JWT Login Endpoint
// Owner: M. Khizar Akram (Team Lead)
// File location: src/Orchestrator/API/Controllers/AuthController.cs
// ============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    // ── Hardcoded test user for project demo ──────────────────
    // In a real system this would check a database
    private const string TEST_USERNAME = "admin";
    private const string TEST_PASSWORD = "intelliflow123";

    /// <summary>
    /// Login and get a JWT token.
    /// POST /api/auth/login
    /// Body: { "username": "admin", "password": "intelliflow123" }
    /// Returns: { "token": "eyJ..." }
    /// Use this token in Postman as: Authorization → Bearer Token
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Step 1 — validate credentials
        if (request.Username != TEST_USERNAME || request.Password != TEST_PASSWORD)
        {
            return Unauthorized(new { error = "Invalid username or password." });
        }

        // Step 2 — read JWT config from environment variables
        var secret   = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? throw new Exception("JWT_SECRET env var not set");
        var issuer   = Environment.GetEnvironmentVariable("JWT_ISSUER")   ?? "IntelliFlow";
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "IntelliFlowClients";

        // Step 3 — build the token
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name,           request.Username),
            new Claim(ClaimTypes.Role,           "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(8),   // token valid for 8 hours
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Step 4 — return token to client
        return Ok(new
        {
            token     = tokenString,
            expiresIn = "8 hours",
            username  = request.Username
        });
    }

    /// <summary>
    /// Quick health check — no auth required
    /// GET /api/auth/health
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "Auth OK" });
}

// ── Request model ────────────────────────────────────────────
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}