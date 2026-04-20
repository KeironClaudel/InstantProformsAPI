namespace InstantProforms.Domain.Common;

/// <summary>
/// Represents a base entity with audit information.
/// </summary>
public abstract class BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the creation date in UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the last update date in UTC.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }
}