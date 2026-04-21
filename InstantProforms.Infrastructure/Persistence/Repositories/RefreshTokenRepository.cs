using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="RefreshToken"/> entities.
/// </summary>
public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RefreshToken?> GetByTokenWithUserAsync(string token, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }
}