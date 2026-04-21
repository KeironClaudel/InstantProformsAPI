using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="User"/> entities.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AnyAsync(x => x.Email == email, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetActiveByEmailWithRoleAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email && x.IsActive, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetActiveByIdWithRoleAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId && x.IsActive, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }
}