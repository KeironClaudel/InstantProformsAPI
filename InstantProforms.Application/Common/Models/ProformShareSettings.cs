namespace InstantProforms.Application.Common.Models;

/// <summary>
/// Represents configuration settings for proform share links.
/// </summary>
public sealed class ProformShareSettings
{
    /// <summary>
    /// Gets or sets the public download base URL.
    /// </summary>
    public string PublicDownloadUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default token expiration in minutes.
    /// </summary>
    public int DefaultExpirationMinutes { get; set; }
}