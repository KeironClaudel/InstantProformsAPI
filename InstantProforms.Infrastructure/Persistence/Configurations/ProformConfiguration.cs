using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the <see cref="Proform"/> entity.
/// </summary>
public sealed class ProformConfiguration : IEntityTypeConfiguration<Proform>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Proform> builder)
    {
        builder.ToTable("Proforms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Number)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Currency)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ClientName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ClientEmail)
            .HasMaxLength(200);

        builder.Property(x => x.ClientPhone)
            .HasMaxLength(50);

        builder.Property(x => x.ClientIdentificationNumberEncrypted)
            .HasMaxLength(2000);

        builder.Property(x => x.Location)
            .HasMaxLength(1000);

        builder.Property(x => x.InternalNotes)
            .HasColumnType("text");

        builder.Property(x => x.ServiceDescription)
            .HasColumnType("text");

        builder.Property(x => x.ScopeOfWork)
            .HasColumnType("text");

        builder.Property(x => x.ServiceConditions)
            .HasColumnType("text");

        builder.Property(x => x.PaymentConditions)
            .HasColumnType("text");

        builder.Property(x => x.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.Total)
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxPercentage)
            .HasPrecision(5, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new { x.CompanyId, x.Number })
            .IsUnique();

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Client)
            .WithMany(x => x.Proforms)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.proforms)
            .HasForeignKey(x => x.ProformId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
