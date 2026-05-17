namespace InstantProforms.Application.Common.Models;

/// <summary>
/// Represents JWT configuration settings.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    /// Gets or sets the issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the signing key.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the access token lifetime in minutes.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the refresh token lifetime in days.
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; }

    /// <summary>
    /// Gets or sets the refresh token lifetime in days for persistent remember-me sessions.
    /// </summary>
    public int? RememberMeRefreshTokenExpirationDays { get; set; }
}
