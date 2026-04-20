using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence;

/// <summary>
/// Represents the application's database context.
/// </summary>
public sealed class AppDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the companies table.
    /// </summary>
    public DbSet<Company> Companies => Set<Company>();

    /// <summary>
    /// Gets or sets the users table.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Configures the model for the database.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}