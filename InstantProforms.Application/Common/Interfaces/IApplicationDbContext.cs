using Microsoft.EntityFrameworkCore;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for the application's database context.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the companies set.
    /// </summary>
    DbSet<Company> Companies { get; }

    /// <summary>
    /// Gets the users set.
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Saves all changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}