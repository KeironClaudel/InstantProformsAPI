using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="Proform"/> entities.
/// </summary>
public interface IProformRepository
{
    /// <summary>
    /// Adds a proforms to the persistence context.
    /// </summary>
    /// <param name="proforms">The proforms to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(Proform proforms, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a proforms by identifier for a specific company, including items.
    /// </summary>
    /// <param name="proformsId">The proforms identifier.</param>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching proforms if found; otherwise, <c>null</c>.</returns>
    Task<Proform?> GetByIdWithItemsAsync(Guid proformsId, Guid companyId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the latest proforms for a company ordered by creation date descending.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The latest proforms if found; otherwise, <c>null</c>.</returns>
    Task<Proform?> GetLatestByCompanyAsync(Guid companyId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a paginated list of Proforms for a company.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A tuple containing items and total count.</returns>
    Task<(IReadOnlyList<Proform> Items, int TotalCount)> GetPagedAsync(
        Guid companyId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}