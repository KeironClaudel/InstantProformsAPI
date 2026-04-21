using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="Role"/> entities.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Gets an active role by identifier.
    /// </summary>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching role if found; otherwise, <c>null</c>.</returns>
    Task<Role?> GetActiveByIdAsync(Guid roleId, CancellationToken cancellationToken);
}