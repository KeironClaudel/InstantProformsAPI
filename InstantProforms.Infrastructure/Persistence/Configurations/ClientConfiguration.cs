using InstantProforms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstantProforms.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the <see cref="Client"/> entity.
/// </summary>
public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.IdentificationNumberEncrypted)
            .HasMaxLength(2000);

        builder.Property(x => x.IdentificationNumberHash)
            .HasMaxLength(128);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => new { x.CompanyId, x.Name });

        builder.HasIndex(x => new { x.CompanyId, x.IdentificationType, x.IdentificationNumberHash })
            .HasFilter("\"IdentificationNumberHash\" IS NOT NULL")
            .IsUnique();

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Clients)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
