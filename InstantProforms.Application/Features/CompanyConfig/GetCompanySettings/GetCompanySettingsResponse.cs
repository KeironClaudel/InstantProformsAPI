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
    string? LogoUrl,
    string? PrimaryColor,
    string? SecondaryColor,
    string? AccentColor,
    string ProformPrefix,
    decimal TaxPercentage,
    string CurrencySymbol,
    string TaxLabel,
    bool HasResendApiKeyConfigured,
    bool IsResendEmailDeliveryConfigured,
    string? ResendSenderEmail,
    string? ResendSenderName,
    string? ResendReplyToEmail);
