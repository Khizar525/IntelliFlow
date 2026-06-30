// ============================================================
// Module 3: Reporter Service
// Owner: Hassan Asif
// ============================================================
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrchestrator", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<IntelliFlowDbContext>(options =>
    options.UseNpgsql(
        Environment.GetEnvironmentVariable("SUPABASE_DB_CONNECTION")));

builder.Services.AddScoped<ReportService>();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowOrchestrator");
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
