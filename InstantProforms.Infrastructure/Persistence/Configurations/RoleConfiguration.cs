using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InstantProforms.Domain.Common;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(250);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasData(
            new Role
            {
                Id = RoleIds.Owner,
                Name = "Owner",
                Description = "Company owner with full access.",
                IsActive = true,
                CreatedAtUtc = new DateTime(2026, 4, 20, 0, 0, 0, DateTimeKind.Utc)
            },
            new Role
            {
                Id = RoleIds.Admin,
                Name = "Admin",
                Description = "Administrator with elevated permissions.",
                IsActive = true,
                CreatedAtUtc = new DateTime(2026, 4, 20, 0, 0, 0, DateTimeKind.Utc)
            },
            new Role
            {
                Id = RoleIds.Employee,
                Name = "Employee",
                Description = "Standard employee user.",
                IsActive = true,
                CreatedAtUtc = new DateTime(2026, 4, 20, 0, 0, 0, DateTimeKind.Utc)
            });
    }
}