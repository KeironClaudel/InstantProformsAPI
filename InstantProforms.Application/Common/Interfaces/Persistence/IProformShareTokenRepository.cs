using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="ProformShareToken"/> entities.
/// </summary>
public interface IProformShareTokenRepository
{
    /// <summary>
    /// Adds a proform share token to the persistence context.
    /// </summary>
    /// <param name="shareToken">The share token entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(ProformShareToken shareToken, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a share token by its hashed token value, including the related proform.
    /// </summary>
    /// <param name="tokenHash">The hashed token value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching share token if found; otherwise, <c>null</c>.</returns>
    Task<ProformShareToken?> GetByTokenHashWithProformAsync(string tokenHash, CancellationToken cancellationToken);
}