using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="Company"/> entities.
/// </summary>
public interface ICompanyRepository
{
    /// <summary>
    /// Determines whether a company slug already exists.
    /// </summary>
    /// <param name="slug">The company slug.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the slug exists; otherwise, <c>false</c>.</returns>
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new company entity to the persistence context.
    /// </summary>
    /// <param name="company">The company to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(Company company, CancellationToken cancellationToken);
}