using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="RefreshToken"/> entities.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Adds a new refresh token entity to the persistence context.
    /// </summary>
    /// <param name="refreshToken">The refresh token to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a refresh token by its token value, including the related user and role.
    /// </summary>
    /// <param name="token">The refresh token value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching refresh token if found; otherwise, <c>null</c>.</returns>
    Task<RefreshToken?> GetByTokenWithUserAsync(string token, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a refresh token by its token value.
    /// </summary>
    /// <param name="token">The refresh token value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching refresh token if found; otherwise, <c>null</c>.</returns>
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
}