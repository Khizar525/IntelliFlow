var builder = WebApplication.CreateBuilder(args);

// Load .env file manually if present (for local dev)
var envFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../.env");
if (File.Exists(envFile))
{
    foreach (var line in File.ReadAllLines(envFile))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
        var parts = line.Split('=', 2);
        if (parts.Length == 2)
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
    }
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services with HttpClient
builder.Services.AddHttpClient<ResearchSummarizer.API.Services.ResearchService>();
builder.Services.AddHttpClient<ResearchSummarizer.API.Services.SummarizerService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();