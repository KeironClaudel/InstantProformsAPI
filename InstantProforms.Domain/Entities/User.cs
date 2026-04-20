using InstantProforms.Domain.Common;

namespace InstantProforms.Domain.Entities;

/// <summary>
/// Represents an application user.
/// </summary>
public sealed class User : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the company identifier.
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password hash.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the user is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the related company.
    /// </summary>
    public Company Company { get; set; } = null!;

    /// <summary>
    /// Gets or sets the related role.
    /// </summary>
    public Role Role { get; set; } = null!;
}