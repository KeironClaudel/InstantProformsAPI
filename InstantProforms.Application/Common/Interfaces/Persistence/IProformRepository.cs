using InstantProforms.Domain.Entities;
using InstantProforms.Domain.Enums;

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
    /// Gets a proform by identifier for a specific company.
    /// </summary>
    /// <param name="proformId">The proform identifier.</param>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching proform if found; otherwise, <c>null</c>.</returns>
    Task<Proform?> GetByIdAsync(Guid proformId, Guid companyId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a paginated list of Proforms for a company.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="clientName">Optional client name filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="issuedFromUtc">Optional issued-at lower bound in UTC.</param>
    /// <param name="issuedToUtc">Optional issued-at upper bound in UTC.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A tuple containing items and total count.</returns>
    Task<(IReadOnlyList<Proform> Items, int TotalCount)> GetPagedAsync(
        Guid companyId,
        int page,
        int pageSize,
        string? clientName,
        ProformStatus? status,
        DateTime? issuedFromUtc,
        DateTime? issuedToUtc,
        CancellationToken cancellationToken);
}
