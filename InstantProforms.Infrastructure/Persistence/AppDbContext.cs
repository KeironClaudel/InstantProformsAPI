using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Proform> Proforms => Set<Proform>();
    public DbSet<ProformItem> ProformItems => Set<ProformItem>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<ProformShareToken> ProformShareTokens => Set<ProformShareToken>();
    public DbSet<CompanySettings> CompanySettings => Set<CompanySettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}