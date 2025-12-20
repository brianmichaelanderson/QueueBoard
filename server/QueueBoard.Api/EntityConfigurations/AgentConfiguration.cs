using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueBoard.Api.Entities;

namespace QueueBoard.Api.EntityConfigurations
{
    public class AgentConfiguration : IEntityTypeConfiguration<Agent>
    {
        public void Configure(EntityTypeBuilder<Agent> builder)
        {
            builder.ToTable("Agents");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(a => a.Email).IsUnique();

            builder.Property(a => a.IsActive)
                .HasDefaultValue(true);

            builder.Property(a => a.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(a => a.UpdatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
