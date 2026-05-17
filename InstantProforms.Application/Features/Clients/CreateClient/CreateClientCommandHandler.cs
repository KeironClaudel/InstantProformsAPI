using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Security;
using InstantProforms.Domain.Entities;
using MediatR;

namespace InstantProforms.Application.Features.Clients.CreateClient;

/// <summary>
/// Handles client creation.
/// </summary>
public sealed class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecretProtector _secretProtector;
    private readonly ISecretFingerprintService _secretFingerprintService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateClientCommandHandler"/> class.
    /// </summary>
    public CreateClientCommandHandler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ISecretProtector secretProtector,
        ISecretFingerprintService secretFingerprintService)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _secretProtector = secretProtector;
        _secretFingerprintService = secretFingerprintService;
    }

    /// <inheritdoc />
    public async Task<ClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var companyId = _currentUserService.CompanyId.Value;
        var normalizedIdentificationNumber = SensitiveValueNormalizer.NormalizeIdentificationNumber(request.IdentificationNumber);

        if (await _unitOfWork.Clients.IsIdentificationInUseAsync(
                companyId,
                request.IdentificationType,
                normalizedIdentificationNumber,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException("The client identification is already registered.");
        }

        var utcNow = DateTime.UtcNow;
        var client = new Client
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = request.Name.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            IdentificationType = request.IdentificationType,
            IdentificationNumberEncrypted = ProtectOrNull(normalizedIdentificationNumber),
            IdentificationNumberHash = ComputeFingerprintOrNull(normalizedIdentificationNumber),
            IdentificationNumber = normalizedIdentificationNumber,
            IsActive = true,
            CreatedAtUtc = utcNow
        };

        await _unitOfWork.Clients.AddAsync(client, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClientResponse(
            client.Id,
            client.Name,
            client.Email,
            client.Phone,
            client.IdentificationType?.ToString(),
            client.IdentificationNumber);
    }

    private string? ProtectOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : _secretProtector.Protect(value);
    }

    private string? ComputeFingerprintOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : _secretFingerprintService.ComputeFingerprint(value);
    }
}
