using Notifier.API.Services;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<BlockchainService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();