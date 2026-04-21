using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Coordinates repositories and persistence operations using a shared database context.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="companies">The company repository.</param>
    /// <param name="users">The user repository.</param>
    /// <param name="roles">The role repository.</param>
    /// <param name="refreshTokens">The refresh token repository.</param>
    /// <param name="passwordResetTokens">The password reset token repository.</param>
    /// <param name="proforms">The proform repository.</param>
    /// <param name="companySettings">The company settings repository.</param>
    public UnitOfWork(
        AppDbContext context,
        ICompanyRepository companies,
        IUserRepository users,
        IRoleRepository roles,
        IRefreshTokenRepository refreshTokens,
        IProformRepository proforms,
        IPasswordResetTokenRepository passwordResetTokens,
        ICompanySettingsRepository companySettings,
        IProformShareTokenRepository proformShareTokens)
    {
        _context = context;
        Companies = companies;
        Users = users;
        Roles = roles;
        RefreshTokens = refreshTokens;
        PasswordResetTokens = passwordResetTokens;
        Proforms = proforms;
        ProformShareTokens = proformShareTokens;
        CompanySettings = companySettings;
    }

    /// <inheritdoc />
    public IProformRepository Proforms { get; }

    /// <inheritdoc />
    public ICompanyRepository Companies { get; }

    /// <inheritdoc />
    public IUserRepository Users { get; }

    /// <inheritdoc />
    public IRoleRepository Roles { get; }

    /// <inheritdoc />
    public IRefreshTokenRepository RefreshTokens { get; }

    /// <inheritdoc />
    public IPasswordResetTokenRepository PasswordResetTokens { get; }

    /// <inheritdoc />
    public IProformShareTokenRepository ProformShareTokens { get; }

    // <inheritdoc />
    public ICompanySettingsRepository CompanySettings { get; }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}