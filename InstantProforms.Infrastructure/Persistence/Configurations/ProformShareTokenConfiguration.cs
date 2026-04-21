using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the <see cref="ProformShareToken"/> entity.
/// </summary>
public sealed class ProformShareTokenConfiguration : IEntityTypeConfiguration<ProformShareToken>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ProformShareToken> builder)
    {
        builder.ToTable("ProformShareTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasOne(x => x.Proform)
            .WithMany(x => x.ShareTokens)
            .HasForeignKey(x => x.ProformId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}