// ============================================================
// Module 2: Research & Summarizer Service
// Owner: Hamza Khaliq
// ============================================================
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

// Register services with HttpClient
builder.Services.AddHttpClient<ResearchSummarizer.API.Services.ResearchService>();
builder.Services.AddHttpClient<ResearchSummarizer.API.Services.SummarizerService>();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
