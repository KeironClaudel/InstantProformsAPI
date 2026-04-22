using InstantProforms.Application.Features.CompanyConfig.UpsertCompanySettings;

namespace InstantProforms.Api.Contracts.CompanySettings;

/// <summary>
/// Provides mappings for company settings contracts.
/// </summary>
public static class UpsertCompanySettingsMappings
{
    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The application command.</returns>
    public static UpsertCompanySettingsCommand ToCommand(this UpsertCompanySettingsRequest request)
    {
        return new UpsertCompanySettingsCommand(
            request.DisplayName,
            request.LegalName,
            request.Website,
            request.Phone,
            request.Email,
            request.Address,
            request.TermsAndConditions,
            request.LogoFileName,
            request.PrimaryColor,
            request.SecondaryColor,
            request.TaxPercentage,
            request.AccentColor,
            request.ProformPrefix,
            request.CurrencySymbol,
            request.TaxLabel);
    }
}