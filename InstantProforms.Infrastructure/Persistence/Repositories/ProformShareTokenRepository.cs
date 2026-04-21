using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="ProformShareToken"/> entities.
/// </summary>
public sealed class ProformShareTokenRepository : IProformShareTokenRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProformShareTokenRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ProformShareTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(ProformShareToken shareToken, CancellationToken cancellationToken)
    {
        await _context.ProformShareTokens.AddAsync(shareToken, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ProformShareToken?> GetByTokenHashWithProformAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _context.ProformShareTokens
            .Include(x => x.Proform)
            .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ProformShareToken?> GetActiveByIdAsync(
        Guid shareTokenId,
        Guid proformId,
        CancellationToken cancellationToken)
    {
        return await _context.ProformShareTokens
            .FirstOrDefaultAsync(
                x => x.Id == shareTokenId &&
                     x.ProformId == proformId &&
                     x.ExpiresAtUtc > DateTime.UtcNow &&
                     (!x.IsSingleUse || x.UsedAtUtc == null),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProformShareToken>> GetActiveByProformIdAsync(
        Guid proformId,
        CancellationToken cancellationToken)
    {
        return await _context.ProformShareTokens
            .Where(x => x.ProformId == proformId &&
                        x.ExpiresAtUtc > DateTime.UtcNow &&
                        (!x.IsSingleUse || x.UsedAtUtc == null))
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}