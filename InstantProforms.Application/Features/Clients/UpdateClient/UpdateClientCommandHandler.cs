using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Security;
using MediatR;

namespace InstantProforms.Application.Features.Clients.UpdateClient;

/// <summary>
/// Handles client updates.
/// </summary>
public sealed class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ClientResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecretProtector _secretProtector;
    private readonly ISecretFingerprintService _secretFingerprintService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateClientCommandHandler"/> class.
    /// </summary>
    public UpdateClientCommandHandler(
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
    public async Task<ClientResponse> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var companyId = _currentUserService.CompanyId.Value;
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId, companyId, cancellationToken);

        if (client is null)
        {
            throw new InvalidOperationException("Client was not found.");
        }

        var normalizedIdentificationNumber = SensitiveValueNormalizer.NormalizeIdentificationNumber(request.IdentificationNumber);

        if (await _unitOfWork.Clients.IsIdentificationInUseAsync(
                companyId,
                request.IdentificationType,
                normalizedIdentificationNumber,
                request.ClientId,
                cancellationToken))
        {
            throw new InvalidOperationException("The client identification is already registered.");
        }

        client.Name = request.Name.Trim();
        client.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        client.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        client.IdentificationType = request.IdentificationType;
        client.IdentificationNumberEncrypted = ProtectOrNull(normalizedIdentificationNumber);
        client.IdentificationNumberHash = ComputeFingerprintOrNull(normalizedIdentificationNumber);
        client.IdentificationNumber = normalizedIdentificationNumber;
        client.UpdatedAtUtc = DateTime.UtcNow;

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
