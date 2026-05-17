using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Interfaces;
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
    private readonly ISecretProtector _secretProtector;
    private readonly ISecretFingerprintService _secretFingerprintService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ClientRepository(
        AppDbContext context,
        ISecretProtector secretProtector,
        ISecretFingerprintService secretFingerprintService)
    {
        _context = context;
        _secretProtector = secretProtector;
        _secretFingerprintService = secretFingerprintService;
    }

    /// <inheritdoc />
    public async Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        await _context.Clients.AddAsync(client, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Client?> GetByIdAsync(Guid clientId, Guid companyId, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(
                x => x.Id == clientId && x.CompanyId == companyId && x.IsActive,
                cancellationToken);

        HydrateSensitiveFields(client);
        return client;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Client>> GetActiveByCompanyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var clients = await _context.Clients
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var client in clients)
        {
            HydrateSensitiveFields(client);
        }

        return clients;
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

        var normalizedHash = _secretFingerprintService.ComputeFingerprint(normalizedIdentificationNumber);

        var query = _context.Clients
            .Where(x =>
                x.CompanyId == companyId &&
                x.IsActive &&
                x.IdentificationType == identificationType);

        if (excludedClientId.HasValue)
        {
            query = query.Where(x => x.Id != excludedClientId.Value);
        }

        var candidates = await query
            .Select(x => x.IdentificationNumberHash)
            .ToListAsync(cancellationToken);

        return candidates.Any(x =>
            string.Equals(x, normalizedHash, StringComparison.OrdinalIgnoreCase));
    }

    private void HydrateSensitiveFields(Client? client)
    {
        if (client is null)
        {
            return;
        }

        client.IdentificationNumber = ResolveSensitiveValue(
            client.IdentificationNumberEncrypted);
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
