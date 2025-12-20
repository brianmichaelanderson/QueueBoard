using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueBoard.Api.Entities;

namespace QueueBoard.Api.EntityConfigurations
{
    public class AgentQueueConfiguration : IEntityTypeConfiguration<AgentQueue>
    {
        public void Configure(EntityTypeBuilder<AgentQueue> builder)
        {
            builder.ToTable("AgentQueues");

            builder.HasKey(aq => new { aq.AgentId, aq.QueueId });

            builder.HasOne(aq => aq.Agent)
                .WithMany(a => a.AgentQueues)
                .HasForeignKey(aq => aq.AgentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(aq => aq.Queue)
                .WithMany(q => q.AgentQueues)
                .HasForeignKey(aq => aq.QueueId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(aq => aq.IsPrimary)
                .HasDefaultValue(false);

            builder.Property(aq => aq.SkillLevel)
                .HasDefaultValue(0);

            builder.Property(aq => aq.AssignedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
