using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="Proform"/> entities.
/// </summary>
public sealed class ProformRepository : IProformRepository
{
    private readonly AppDbContext _context;
    private readonly ISecretProtector _secretProtector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProformRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="secretProtector">The reversible secret protector.</param>
    public ProformRepository(AppDbContext context, ISecretProtector secretProtector)
    {
        _context = context;
        _secretProtector = secretProtector;
    }

    /// <inheritdoc />
    public async Task AddAsync(Proform proform, CancellationToken cancellationToken)
    {
        await _context.Proforms.AddAsync(proform, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Proform?> GetByIdWithItemsAsync(
        Guid proformsId,
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var proform = await _context.Proforms
            .Include(x => x.Items.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(
                x => x.Id == proformsId && x.CompanyId == companyId,
                cancellationToken);

        HydrateSensitiveFields(proform);
        return proform;
    }

    /// <inheritdoc />
    public async Task<Proform?> GetLatestByCompanyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var proform = await _context.Proforms
            .Where(x => x.CompanyId == companyId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        HydrateSensitiveFields(proform);
        return proform;
    }

    /// <inheritdoc />
    public async Task<Proform?> GetByIdAsync(Guid proformId, Guid companyId, CancellationToken cancellationToken)
    {
        var proform = await _context.Proforms
            .FirstOrDefaultAsync(
                x => x.Id == proformId && x.CompanyId == companyId,
                cancellationToken);

        HydrateSensitiveFields(proform);
        return proform;
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<Proform> Items, int TotalCount)> GetPagedAsync(
        Guid companyId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Proforms
            .Where(x => x.CompanyId == companyId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            HydrateSensitiveFields(item);
        }

        return (items, totalCount);
    }

    private void HydrateSensitiveFields(Proform? proform)
    {
        if (proform is null)
        {
            return;
        }

        proform.ClientIdentificationNumber = ResolveSensitiveValue(
            proform.ClientIdentificationNumberEncrypted);
    }

    private string? ResolveSensitiveValue(string? encryptedValue)
    {
        if (!string.IsNullOrWhiteSpace(encryptedValue))
        {
            return _secretProtector.Unprotect(encryptedValue);
        }

        return null;
    }
}
