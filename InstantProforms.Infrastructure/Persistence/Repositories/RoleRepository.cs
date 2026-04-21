using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="Role"/> entities.
/// </summary>
public sealed class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Role?> GetActiveByIdAsync(Guid roleId, CancellationToken cancellationToken)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x => x.Id == roleId && x.IsActive, cancellationToken);
    }
}