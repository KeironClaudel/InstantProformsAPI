using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="Company"/> entities.
/// </summary>
public sealed class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CompanyRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken)
    {
        return await _context.Companies
            .AnyAsync(x => x.Slug == slug, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Company company, CancellationToken cancellationToken)
    {
        await _context.Companies.AddAsync(company, cancellationToken);
    }
}