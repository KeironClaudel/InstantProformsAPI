
namespace InstantProforms.Api.Contracts.Auth;

/// <summary>
/// Represents the HTTP request to register a company with branding settings.
/// </summary>
public sealed class RegisterCompanyRequest
{
    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company slug.
    /// </summary>
    public string CompanySlug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company email.
    /// </summary>
    public string? CompanyEmail { get; set; }

    /// <summary>
    /// Gets or sets the company phone.
    /// </summary>
    public string? CompanyPhone { get; set; }

    /// <summary>
    /// Gets or sets the company address.
    /// </summary>
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// Gets or sets the company website.
    /// </summary>
    public string? CompanyWebsite { get; set; }

    /// <summary>
    /// Gets or sets the display name used in documents.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the legal name.
    /// </summary>
    public string? LegalName { get; set; }

    /// <summary>
    /// Gets or sets the terms and conditions.
    /// </summary>
    public string TermsAndConditions { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the primary branding color.
    /// </summary>
    public string PrimaryColor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secondary branding color.
    /// </summary>
    public string SecondaryColor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the accent branding color.
    /// </summary>
    public string AccentColor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the proform prefix.
    /// </summary>
    public string ProformPrefix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the currency symbol.
    /// </summary>
    public string CurrencySymbol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tax label.
    /// </summary>
    public string TaxLabel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company logo file.
    /// </summary>
    public IFormFile? LogoFile { get; set; }

    /// <summary>
    /// Gets or sets the owner full name.
    /// </summary>
    public string OwnerFullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owner email.
    /// </summary>
    public string OwnerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tax percentage.
    /// </summary>
    public decimal TaxPercentage { get; set; }
}
