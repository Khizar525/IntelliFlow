// ============================================================
// Module 1: Orchestrator API Gateway
// Owner: M. Khizar Akram (Team Lead)
// ============================================================
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

// Explicitly specify the path to the .env file
Env.Load("D:\\cloud_project\\IntelliFlow\\.env");

// Debug statement to verify JWT_SECRET
Console.WriteLine($"JWT_SECRET: {Environment.GetEnvironmentVariable("JWT_SECRET")}");

// Load environment variables from .env file
Env.Load();

// Access JWT_SECRET
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new Exception("JWT_SECRET env var not set");
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

// HttpClients for calling sub-agents
builder.Services.AddHttpClient("Research",  c => c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("RESEARCH_SERVICE_URL")!));
builder.Services.AddHttpClient("Reporter",  c => c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("REPORTER_SERVICE_URL")!));
builder.Services.AddHttpClient("Notifier",  c => c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("NOTIFIER_SERVICE_URL")!));

builder.Services.AddScoped<OrchestratorService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
