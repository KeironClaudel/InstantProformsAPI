namespace InstantProforms.Application.Common.Models;

/// <summary>
/// Stores platform-level administration settings.
/// </summary>
public sealed class PlatformAdminSettings
{
    /// <summary>
    /// Gets or sets the normalized email addresses allowed to access platform administration features.
    /// </summary>
    public List<string> AllowedEmails { get; set; } = new();
}
