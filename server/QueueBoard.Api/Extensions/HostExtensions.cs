using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueueBoard.Api;
using Microsoft.Extensions.Logging;
using QueueBoard.Api.Data;

namespace QueueBoard.Api.Extensions
{
    public static class HostExtensions
    {
        public static void SeedDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetService<ILogger<QueueBoardDbContext>>();

            try
            {
                var context = services.GetRequiredService<QueueBoardDbContext>();
                try
                {
                    // Ensure database is created for development convenience.
                    context.Database.EnsureCreated();

                    // If there are migrations, apply them (no-op if none).
                    var pending = context.Database.GetPendingMigrations();
                    if (pending != null && pending.Any())
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception migrateEx)
                {
                    logger?.LogWarning(migrateEx, "Database migration failed, falling back to EnsureCreated().");
                    context.Database.EnsureCreated();
                }

                SeedData.EnsureSeeded(context);
                logger?.LogInformation("Database migrated and seeded.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw;
            }
        }
    }
}
