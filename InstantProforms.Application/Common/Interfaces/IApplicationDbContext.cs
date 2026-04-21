using Microsoft.EntityFrameworkCore;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Proform> Proforms { get; }
    DbSet<ProformItem> ProformItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}