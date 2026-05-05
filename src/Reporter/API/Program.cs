// Module 3 — Owner: Hassan Asif
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO Hassan: Register DbContext when EF Core is wired up
// builder.Services.AddDbContext<IntelliFlowDbContext>(options =>
//     options.UseSqlServer(Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION")));

builder.Services.AddScoped<ReportService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
