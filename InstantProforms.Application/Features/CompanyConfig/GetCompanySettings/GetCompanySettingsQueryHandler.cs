using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;

namespace InstantProforms.Application.Features.CompanyConfig.GetCompanySettings;

/// <summary>
/// Handles retrieval of the current company settings.
/// </summary>
public sealed class GetCompanySettingsQueryHandler
    : IRequestHandler<GetCompanySettingsQuery, GetCompanySettingsResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ISecretProtector _secretProtector;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCompanySettingsQueryHandler"/> class.
    /// </summary>
    public GetCompanySettingsQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        ISecretProtector secretProtector)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _secretProtector = secretProtector;
    }

    /// <inheritdoc />
    public async Task<GetCompanySettingsResponse> Handle(
        GetCompanySettingsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var settings = await _unitOfWork.CompanySettings
            .GetByCompanyIdAsync(_currentUserService.CompanyId.Value, cancellationToken);

        if (settings is null)
        {
            throw new InvalidOperationException("Company settings were not found.");
        }

        return new GetCompanySettingsResponse(
            settings.DisplayName,
            settings.LegalName,
            settings.Website,
            settings.Phone,
            settings.Email,
            settings.Address,
            settings.TermsAndConditions,
            settings.LogoFileName,
            _fileStorageService.GetPublicUrl(settings.LogoFileName),
            settings.PrimaryColor,
            settings.SecondaryColor,
            settings.AccentColor,
            settings.ProformPrefix,
            settings.TaxPercentage,
            settings.CurrencySymbol,
            settings.TaxLabel,
            !string.IsNullOrWhiteSpace(settings.ResendApiKeyEncrypted),
            !string.IsNullOrWhiteSpace(settings.ResendApiKeyEncrypted)
                && !string.IsNullOrWhiteSpace(settings.ResendSenderEmailEncrypted),
            UnprotectOrNull(settings.ResendSenderEmailEncrypted),
            UnprotectOrNull(settings.ResendSenderNameEncrypted),
            UnprotectOrNull(settings.ResendReplyToEmailEncrypted));
    }

    private string? UnprotectOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : _secretProtector.Unprotect(value);
    }
}
