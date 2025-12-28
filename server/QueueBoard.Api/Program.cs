using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QueueBoard.Api;
using QueueBoard.Api.Extensions;

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

var xmlFile = (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "QueueBoard.Api") + ".xml";
var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "QueueBoard API",
        Version = "v1",
        Description = "QueueBoard ASP.NET Core API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "QueueBoard",
            Email = "dev@example.com"
        }
    });

    if (System.IO.File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Apply migrations and seed DB in development/local environments (automated)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure database is migrated and seeded on startup (idempotent)
try
{
    app.SeedDatabase();
}
catch
{
    // If seeding fails, let the exception bubble to stop startup so the issue can be addressed.
    throw;
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
