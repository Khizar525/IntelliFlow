using Notifier.API.Services;

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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<BlockchainService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();