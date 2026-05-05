// Module 2 — Owner: Hamza Khaliq
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ResearchService>();
builder.Services.AddScoped<SummarizerService>(sp =>
    new SummarizerService(sp.GetRequiredService<IHttpClientFactory>().CreateClient()));
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
