namespace InstantProforms.Application.Common.Models;

/// <summary>
/// Represents Resend configuration settings.
/// </summary>
public sealed class ResendSettings
{
    /// <summary>
    /// Gets or sets the Resend API base URL.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.resend.com/";

    /// <summary>
    /// Gets or sets the legacy global Resend API key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the legacy global sender display name.
    /// </summary>
    public string SenderName { get; set; } = "InstantQuotations";

    /// <summary>
    /// Gets or sets the legacy global sender email address.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional legacy global reply-to email address.
    /// </summary>
    public string? ReplyToEmail { get; set; }
}
