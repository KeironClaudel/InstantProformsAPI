using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="Proform"/> entities.
/// </summary>
public sealed class ProformRepository : IProformRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProformRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ProformRepository(AppDbContext context)
    {
        _context = context;
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
        return await _context.Proforms
            .Include(x => x.Items.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(
                x => x.Id == proformsId && x.CompanyId == companyId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Proform?> GetLatestByCompanyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await _context.Proforms
            .Where(x => x.CompanyId == companyId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Proform?> GetByIdAsync(Guid proformId, Guid companyId, CancellationToken cancellationToken)
    {
        return await _context.Proforms
            .FirstOrDefaultAsync(
                x => x.Id == proformId && x.CompanyId == companyId,
                cancellationToken);
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

        return (items, totalCount);
    }
}