namespace InstantProforms.Application.Common.Models;

/// <summary>
/// Represents password reset configuration settings.
/// </summary>
public sealed class PasswordResetSettings
{
    /// <summary>
    /// Gets or sets the frontend reset password URL.
    /// </summary>
    public string ResetUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiration time in minutes.
    /// </summary>
    public int TokenExpirationMinutes { get; set; }
}