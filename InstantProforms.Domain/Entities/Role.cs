using InstantProforms.Domain.Common;

namespace InstantProforms.Domain.Entities;

/// <summary>
/// Represents a user role in the system.
/// </summary>
public sealed class Role : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the role name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the role is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the users assigned to this role.
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();
}