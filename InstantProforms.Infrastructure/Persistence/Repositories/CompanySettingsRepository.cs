using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="CompanySettings"/> entities.
/// </summary>
public sealed class CompanySettingsRepository : ICompanySettingsRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanySettingsRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CompanySettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<CompanySettings?> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await _context.CompanySettings
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(CompanySettings settings, CancellationToken cancellationToken)
    {
        await _context.CompanySettings.AddAsync(settings, cancellationToken);
    }
}