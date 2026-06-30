// ============================================================
// Module 4: Notifier Service
// Owner: Shamraiz
// ============================================================
using Notifier.API.Services;
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

builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<BlockchainService>();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowOrchestrator");
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
