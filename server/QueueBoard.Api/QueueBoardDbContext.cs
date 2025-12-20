using System.Reflection;
using Microsoft.EntityFrameworkCore;
using QueueBoard.Api.Entities;

namespace QueueBoard.Api
{
    public class QueueBoardDbContext : DbContext
    {
        public QueueBoardDbContext(DbContextOptions<QueueBoardDbContext> options) : base(options)
        {
        }

        public DbSet<Queue> Queues { get; set; } = null!;
        public DbSet<Agent> Agents { get; set; } = null!;
        public DbSet<AgentQueue> AgentQueues { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
