using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="CompanySettings"/> entities.
/// </summary>
public interface ICompanySettingsRepository
{
    /// <summary>
    /// Gets company settings by company identifier.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The company settings if found; otherwise, <c>null</c>.</returns>
    Task<CompanySettings?> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds company settings to the persistence context.
    /// </summary>
    /// <param name="settings">The settings entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(CompanySettings settings, CancellationToken cancellationToken);
}