using MediatR;

namespace InstantProforms.Application.Features.CompanyConfig.UpsertCompanySettings;

/// <summary>
/// Represents a request to create or update company settings.
/// </summary>
public sealed record UpsertCompanySettingsCommand(
    string DisplayName,
    string? LegalName,
    string? Website,
    string? Phone,
    string? Email,
    string? Address,
    string? TermsAndConditions,
    string? LogoFileName,
    string? PrimaryColor,
    string? SecondaryColor,
    string? AccentColor,
    string ProformPrefix,
    string CurrencySymbol,
    string TaxLabel) : IRequest<UpsertCompanySettingsResponse>;