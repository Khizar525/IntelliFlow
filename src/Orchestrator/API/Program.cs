// ============================================================
// Module 1: Orchestrator API Gateway
// Owner: M. Khizar Akram (Team Lead)
// ============================================================
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using Orchestrator.API.Middlewares;

// Load environment variables from .env file
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}
else
{
    // Try relative paths for Docker containers
    var relativePaths = new[] { "../../../.env", "../../../../.env" };
    foreach (var relativePath in relativePaths)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        if (File.Exists(fullPath))
        {
            Env.Load(fullPath);
            break;
        }
    }
}

// Validate required environment variables
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer   = true,
            ValidIssuer      = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidateAudience = true,
            ValidAudience    = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        };
    });

builder.Services.AddAuthorization();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// HttpClients for calling sub-agents with resilience policies
builder.Services.AddHttpClient("Research", c => 
{
    c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("RESEARCH_SERVICE_URL")!);
    c.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(Policy<HttpResponseMessage>.Handle<HttpRequestException>()
    .OrResult(msg => !msg.IsSuccessStatusCode)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

builder.Services.AddHttpClient("Reporter", c => 
{
    c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("REPORTER_SERVICE_URL")!);
    c.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(Policy<HttpResponseMessage>.Handle<HttpRequestException>()
    .OrResult(msg => !msg.IsSuccessStatusCode)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

builder.Services.AddHttpClient("Notifier", c => 
{
    c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("NOTIFIER_SERVICE_URL")!);
    c.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(Policy<HttpResponseMessage>.Handle<HttpRequestException>()
    .OrResult(msg => !msg.IsSuccessStatusCode)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

builder.Services.AddScoped<OrchestratorService>();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Global exception handler (must be first)
app.UseGlobalExceptionHandler();

// Request logging
app.UseRequestLogging();

// Rate limiting
app.UseRateLimiting();

app.UseSwagger();
app.UseSwaggerUI();

// CORS (must be after UseRouting, before UseAuthorization)
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
