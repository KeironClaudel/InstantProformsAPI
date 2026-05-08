using Microsoft.EntityFrameworkCore;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<Client> Clients { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Proform> Proforms { get; }
    DbSet<ProformItem> ProformItems { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }
    DbSet<ProformShareToken> ProformShareTokens { get; }
    DbSet<CompanySettings> CompanySettings { get; }
    DbSet<StoredFile> StoredFiles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
