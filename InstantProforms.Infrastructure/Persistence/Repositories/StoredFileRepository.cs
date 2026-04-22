using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="StoredFile"/> entities.
/// </summary>
public sealed class StoredFileRepository : IStoredFileRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="StoredFileRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public StoredFileRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(StoredFile storedFile, CancellationToken cancellationToken)
    {
        await _context.StoredFiles.AddAsync(storedFile, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.StoredFiles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}