using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces.Persistence;

/// <summary>
/// Defines data access operations for <see cref="StoredFile"/> entities.
/// </summary>
public interface IStoredFileRepository
{
    /// <summary>
    /// Adds a stored file to the persistence context.
    /// </summary>
    /// <param name="storedFile">The stored file entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(StoredFile storedFile, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a stored file by identifier.
    /// </summary>
    /// <param name="id">The stored file identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stored file if found; otherwise, <c>null</c>.</returns>
    Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}