using Microsoft.EntityFrameworkCore;
using QueueBoard.Api;

var builder = WebApplication.CreateBuilder(args);

// Resolve connection string from environment or configuration.
// Priority:
// 1. Environment variable DEFAULT_CONNECTION (full connection string)
// 2. Configuration connection string DefaultConnection
// 3. Build from DB_HOST/DB_PORT/DB_USER/SA_PASSWORD environment variables
string? connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
    var user = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
    var password = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? string.Empty;
    connectionString = $"Server={host},{port};User Id={user};Password={password};TrustServerCertificate=True;";
}

builder.Services.AddDbContext<QueueBoardDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
