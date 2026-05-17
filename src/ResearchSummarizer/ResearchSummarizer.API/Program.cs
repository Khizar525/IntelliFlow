Console.WriteLine($"DEBUG CWD: {Directory.GetCurrentDirectory()}");

var builder = WebApplication.CreateBuilder(args);

// Load .env file — try CWD first, then project-relative paths
var candidates = new[] { ".env", "../../../.env", "../../../../.env" }
    .Select(p => Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), p)))
    .ToArray();
Console.WriteLine($"DEBUG .env candidates: {string.Join(", ", candidates)}");
var envFile = candidates.FirstOrDefault(File.Exists);
Console.WriteLine($"DEBUG .env found: {envFile ?? "NULL"}");
if (envFile != null)
{
    foreach (var line in File.ReadAllLines(envFile))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
        var parts = line.Split('=', 2);
        if (parts.Length == 2)
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
    }
}
Console.WriteLine($"DEBUG OPENROUTER_API_KEY after load: {(Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? "NULL")}");

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