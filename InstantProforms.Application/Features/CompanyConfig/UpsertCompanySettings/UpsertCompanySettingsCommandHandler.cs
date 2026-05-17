using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Features.CompanyConfig.UpsertCompanySettings;

/// <summary>
/// Handles creation or update of company settings.
/// </summary>
public sealed class UpsertCompanySettingsCommandHandler
    : IRequestHandler<UpsertCompanySettingsCommand, UpsertCompanySettingsResponse>
{
    private const string LegacyProformPrefix = "PRO";

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISecretProtector _secretProtector;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpsertCompanySettingsCommandHandler"/> class.
    /// </summary>
    public UpsertCompanySettingsCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ISecretProtector secretProtector)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _secretProtector = secretProtector;
    }

    /// <inheritdoc />
    public async Task<UpsertCompanySettingsResponse> Handle(
        UpsertCompanySettingsCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var companyId = _currentUserService.CompanyId.Value;

        var settings = await _unitOfWork.CompanySettings
            .GetByCompanyIdAsync(companyId, cancellationToken);

        if (settings is null)
        {
            settings = new CompanySettings
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.CompanySettings.AddAsync(settings, cancellationToken);
        }

        settings.DisplayName = request.DisplayName;
        settings.LegalName = request.LegalName;
        settings.Website = request.Website;
        settings.Phone = request.Phone;
        settings.Email = request.Email;
        settings.Address = request.Address;
        settings.TermsAndConditions = request.TermsAndConditions;
        settings.LogoFileName = request.LogoFileName;
        settings.PrimaryColor = request.PrimaryColor;
        settings.SecondaryColor = request.SecondaryColor;
        settings.AccentColor = request.AccentColor;
        settings.ProformPrefix = string.IsNullOrWhiteSpace(request.ProformPrefix)
            ? LegacyProformPrefix
            : request.ProformPrefix.Trim();
        settings.CurrencySymbol = request.CurrencySymbol;
        settings.TaxPercentage = request.TaxPercentage;
        settings.TaxLabel = request.TaxLabel;
        settings.ResendSenderEmailEncrypted = ProtectOrNull(request.ResendSenderEmail);
        settings.ResendSenderNameEncrypted = ProtectOrNull(request.ResendSenderName);
        settings.ResendReplyToEmailEncrypted = ProtectOrNull(request.ResendReplyToEmail);

        if (request.ClearResendApiKey)
        {
            settings.ResendApiKeyEncrypted = null;
        }
        else if (!string.IsNullOrWhiteSpace(request.ResendApiKey))
        {
            settings.ResendApiKeyEncrypted = _secretProtector.Protect(request.ResendApiKey.Trim());
        }

        settings.UpdatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpsertCompanySettingsResponse("Company settings saved successfully.");
    }

    private string? ProtectOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : _secretProtector.Protect(value.Trim());
    }
}
