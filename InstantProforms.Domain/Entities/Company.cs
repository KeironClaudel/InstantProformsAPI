using InstantProforms.Domain.Common;

namespace InstantProforms.Domain.Entities;

/// <summary>
/// Represents a company that uses the platform.
/// </summary>
public sealed class Company : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the public slug for the company.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the contact email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the contact phone.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the company address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the company logo URL.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the company is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the company settings.
    /// </summary>
    public CompanySettings? Settings { get; set; }

    /// <summary>
    /// Gets or sets the users that belong to the company.
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();
}