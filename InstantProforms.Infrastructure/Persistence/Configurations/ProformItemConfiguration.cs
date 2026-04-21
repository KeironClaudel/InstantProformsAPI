using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the <see cref="ProformItem"/> entity.
/// </summary>
public sealed class ProformItemConfiguration : IEntityTypeConfiguration<ProformItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ProformItem> builder)
    {
        builder.ToTable("ProformItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.Total)
            .HasPrecision(18, 2);
    }
}