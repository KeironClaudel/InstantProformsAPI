namespace InstantProforms.Application.Features.CompanyConfig.GetCompanySettings;

/// <summary>
/// Represents company settings data.
/// </summary>
public sealed record GetCompanySettingsResponse(
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
    string TaxLabel);