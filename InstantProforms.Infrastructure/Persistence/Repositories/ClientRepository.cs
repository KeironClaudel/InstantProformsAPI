using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;
using InstantProforms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InstantProforms.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides EF Core data access operations for <see cref="Client"/> entities.
/// </summary>
public sealed class ClientRepository : IClientRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ClientRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        await _context.Clients.AddAsync(client, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Client?> GetByIdAsync(Guid clientId, Guid companyId, CancellationToken cancellationToken)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(
                x => x.Id == clientId && x.CompanyId == companyId && x.IsActive,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Client>> GetActiveByCompanyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await _context.Clients
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsIdentificationInUseAsync(
        Guid companyId,
        ClientIdentificationType? identificationType,
        string? identificationNumber,
        Guid? excludedClientId,
        CancellationToken cancellationToken)
    {
        if (identificationType is null || string.IsNullOrWhiteSpace(identificationNumber))
        {
            return false;
        }

        var normalizedIdentificationNumber = identificationNumber.Trim();

        var query = _context.Clients
            .Where(x =>
                x.CompanyId == companyId &&
                x.IsActive &&
                x.IdentificationType == identificationType &&
                x.IdentificationNumber == normalizedIdentificationNumber);

        if (excludedClientId.HasValue)
        {
            query = query.Where(x => x.Id != excludedClientId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
