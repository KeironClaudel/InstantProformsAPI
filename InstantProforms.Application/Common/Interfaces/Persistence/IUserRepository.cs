using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="User"/> entities.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Determines whether an email already exists in the system.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the email exists; otherwise, <c>false</c>.</returns>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an active user by email, including its role.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching user if found; otherwise, <c>null</c>.</returns>
    Task<User?> GetActiveByEmailWithRoleAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an active user by identifier, including its role.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching user if found; otherwise, <c>null</c>.</returns>
    Task<User?> GetActiveByIdWithRoleAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new user entity to the persistence context.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>

    /// <summary>
    /// Gets a user by email, including its role.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching user if found; otherwise, <c>null</c>.</returns>
    Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}