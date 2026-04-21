using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="PasswordResetToken"/> entities.
/// </summary>
public sealed class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PasswordResetTokenRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public PasswordResetTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(PasswordResetToken passwordResetToken, CancellationToken cancellationToken)
    {
        await _context.PasswordResetTokens.AddAsync(passwordResetToken, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PasswordResetToken?> GetByTokenHashWithUserAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _context.PasswordResetTokens
            .Include(x => x.User)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }
}