using InstantProforms.Domain.Entities;
using InstantProforms.Domain.Enums;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="Client"/> entities.
/// </summary>
public interface IClientRepository
{
    /// <summary>
    /// Adds a client to the persistence context.
    /// </summary>
    /// <param name="client">The client to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(Client client, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an active client by identifier for a specific company.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching client if found; otherwise, <c>null</c>.</returns>
    Task<Client?> GetByIdAsync(Guid clientId, Guid companyId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets active clients for a specific company ordered by name.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The active client list.</returns>
    Task<IReadOnlyList<Client>> GetActiveByCompanyAsync(Guid companyId, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether an identification value is already used by another active client in the same company.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="identificationType">The identification type.</param>
    /// <param name="identificationNumber">The identification number.</param>
    /// <param name="excludedClientId">An optional client identifier to exclude from the check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the identification is already in use; otherwise, <c>false</c>.</returns>
    Task<bool> IsIdentificationInUseAsync(
        Guid companyId,
        ClientIdentificationType? identificationType,
        string? identificationNumber,
        Guid? excludedClientId,
        CancellationToken cancellationToken);
}
