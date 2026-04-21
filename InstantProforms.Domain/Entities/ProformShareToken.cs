using InstantProforms.Domain.Common;

namespace InstantProforms.Domain.Entities;

/// <summary>
/// Represents a temporary public access token for downloading a proform PDF.
/// </summary>
public sealed class ProformShareToken : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the proform identifier.
    /// </summary>
    public Guid ProformId { get; set; }

    /// <summary>
    /// Gets or sets the hashed token value.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration date in UTC.
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the date when the token was used in UTC.
    /// </summary>
    public DateTime? UsedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token is single use.
    /// </summary>
    public bool IsSingleUse { get; set; }

    /// <summary>
    /// Gets or sets the related proform.
    /// </summary>
    public Proform Proform { get; set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the token is active.
    /// </summary>
    public bool IsActive => ExpiresAtUtc > DateTime.UtcNow && (!IsSingleUse || UsedAtUtc is null);
}