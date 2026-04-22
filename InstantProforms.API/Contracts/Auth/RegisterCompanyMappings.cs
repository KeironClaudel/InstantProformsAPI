using InstantProforms.Application.Features.Auth.RegisterCompany;

namespace InstantProforms.Api.Contracts.Auth;

/// <summary>
/// Provides mappings for company registration contracts.
/// </summary>
public static class RegisterCompanyMappings
{
    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The application command.</returns>
    public static RegisterCompanyCommand ToCommand(this RegisterCompanyRequest request)
    {
        return new RegisterCompanyCommand(
            request.CompanyName,
            request.CompanySlug,
            request.CompanyEmail,
            request.CompanyPhone,
            request.CompanyAddress,
            request.CompanyWebsite,
            request.DisplayName,
            request.LegalName,
            request.TermsAndConditions,
            request.PrimaryColor,
            request.SecondaryColor,
            request.AccentColor,
            request.ProformPrefix,
            request.CurrencySymbol,
            request.TaxLabel,
            request.TaxPercentage,
            request.LogoFile,
            request.OwnerFullName,
            request.OwnerEmail,
            request.Password);
    }
}