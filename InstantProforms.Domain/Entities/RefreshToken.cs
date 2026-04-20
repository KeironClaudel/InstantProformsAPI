using InstantProforms.Domain.Common;

namespace InstantProforms.Domain.Entities;

/// <summary>
/// Represents a refresh token issued to a user.
/// </summary>
public sealed class RefreshToken : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the token value.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration date in UTC.
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the revocation date in UTC.
    /// </summary>
    public DateTime? RevokedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the IP address that created the token.
    /// </summary>
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// Gets or sets the IP address that revoked the token.
    /// </summary>
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Gets a value indicating whether the token is active.
    /// </summary>
    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the related user.
    /// </summary>
    public User User { get; set; } = null!;
}