using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueBoard.Api.Entities;

namespace QueueBoard.Api.EntityConfigurations
{
    public class QueueConfiguration : IEntityTypeConfiguration<Queue>
    {
        public void Configure(EntityTypeBuilder<Queue> builder)
        {
            builder.ToTable("Queues");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(q => q.Name);

            builder.Property(q => q.Description)
                .HasMaxLength(1000);

            builder.Property(q => q.IsActive)
                .HasDefaultValue(true);

            builder.Property(q => q.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            builder.Property(q => q.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(q => q.UpdatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
