using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the <see cref="CompanySettings"/> entity.
/// </summary>
public sealed class CompanySettingsConfiguration : IEntityTypeConfiguration<CompanySettings>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CompanySettings> builder)
    {
        builder.ToTable("CompanySettings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.LegalName)
            .HasMaxLength(200);

        builder.Property(x => x.Website)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.Address)
            .HasMaxLength(300);

        builder.Property(x => x.TermsAndConditions)
            .HasMaxLength(4000);

        builder.Property(x => x.LogoFileName)
            .HasMaxLength(255);

        builder.Property(x => x.PrimaryColor)
            .HasMaxLength(20);

        builder.Property(x => x.SecondaryColor)
            .HasMaxLength(20);

        builder.Property(x => x.AccentColor)
            .HasMaxLength(20);

        builder.Property(x => x.ProformPrefix)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CurrencySymbol)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.TaxLabel)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.CompanyId)
            .IsUnique();

        builder.HasOne(x => x.Company)
            .WithOne(x => x.Settings)
            .HasForeignKey<CompanySettings>(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}