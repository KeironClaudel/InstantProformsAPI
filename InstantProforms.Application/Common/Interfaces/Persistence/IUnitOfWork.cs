namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines a unit of work for coordinating repositories and committing changes.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the company repository.
    /// </summary>
    ICompanyRepository Companies { get; }

    /// <summary>
    /// Gets the user repository.
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Gets the role repository.
    /// </summary>
    IRoleRepository Roles { get; }

    /// <summary>
    /// Gets the refresh token repository.
    /// </summary>
    IRefreshTokenRepository RefreshTokens { get; }

    /// <summary>
    /// Gets the password reset token repository.
    /// </summary>
    IPasswordResetTokenRepository PasswordResetTokens { get; }

    /// <summary>
    /// Gets the proform repository.
    /// </summary>
    IProformRepository Proforms { get; }

    /// <summary>
    /// Persists all pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of affected rows.</returns>

    /// <summary>
    /// Gets the proform share token repository.
    /// </summary>
    IProformShareTokenRepository ProformShareTokens { get; }

    /// <summary>
    /// Gets the company settings repository.
    /// </summary>
    ICompanySettingsRepository CompanySettings { get; }

    /// <summary>
    /// Gets the stored file repository.
    /// </summary>
    IStoredFileRepository StoredFiles { get; }

    /// <summary>
    /// Persists all pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of affected rows.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}