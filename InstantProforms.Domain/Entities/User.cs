using InstantProforms.Domain.Common;

namespace InstantProforms.Domain.Entities;

/// <summary>
/// Represents an application user.
/// </summary>
public sealed class User : BaseAuditableEntity
{
    public Guid CompanyId { get; set; }
    public Guid RoleId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Company Company { get; set; } = null!;
    public Role Role { get; set; } = null!;


    /// <summary>
    /// Gets or sets the password reset tokens issued to the user.
    /// </summary>
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    /// <summary>
    /// Gets or sets the refresh tokens issued to the user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}