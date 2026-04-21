using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="PasswordResetToken"/> entities.
/// </summary>
public interface IPasswordResetTokenRepository
{
    /// <summary>
    /// Adds a password reset token to the persistence context.
    /// </summary>
    /// <param name="passwordResetToken">The token entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(PasswordResetToken passwordResetToken, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a password reset token by hashed token value, including the related user and role.
    /// </summary>
    /// <param name="tokenHash">The hashed token value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching token if found; otherwise, <c>null</c>.</returns>
    Task<PasswordResetToken?> GetByTokenHashWithUserAsync(string tokenHash, CancellationToken cancellationToken);
}