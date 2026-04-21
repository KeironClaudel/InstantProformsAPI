namespace InstantProforms.Api.Contracts.CompanySettings;

/// <summary>
/// Represents the HTTP request to create or update company settings.
/// </summary>
public sealed record UpsertCompanySettingsRequest(
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