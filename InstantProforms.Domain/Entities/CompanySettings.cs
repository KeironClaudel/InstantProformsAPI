using InstantProforms.Domain.Common;

namespace InstantProforms.Domain.Entities;

/// <summary>
/// Represents tenant-specific branding and document settings for a company.
/// </summary>
public sealed class CompanySettings : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the company identifier.
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the display name used in documents.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional legal name.
    /// </summary>
    public string? LegalName { get; set; }

    /// <summary>
    /// Gets or sets the public website.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the primary contact phone.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the contact email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the company address shown in documents.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the terms and conditions used in proforms.
    /// </summary>
    public string? TermsAndConditions { get; set; }

    /// <summary>
    /// Gets or sets the stored logo file name.
    /// </summary>
    public string? LogoFileName { get; set; }

    /// <summary>
    /// Gets or sets the primary branding color.
    /// </summary>
    public string? PrimaryColor { get; set; }

    /// <summary>
    /// Gets or sets the secondary branding color.
    /// </summary>
    public string? SecondaryColor { get; set; }

    /// <summary>
    /// Gets or sets the accent branding color.
    /// </summary>
    public string? AccentColor { get; set; }

    /// <summary>
    /// Gets or sets the proform number prefix.
    /// </summary>
    public string ProformPrefix { get; set; } = "PRO";

    /// <summary>
    /// Gets or sets the currency symbol.
    /// </summary>
    public string CurrencySymbol { get; set; } = "₡";

    /// <summary>
    /// Gets or sets the tax label shown in totals.
    /// </summary>
    public string TaxLabel { get; set; } = "Total";

    /// <summary>
    /// Gets or sets the related company.
    /// </summary>
    public Company Company { get; set; } = null!;
}