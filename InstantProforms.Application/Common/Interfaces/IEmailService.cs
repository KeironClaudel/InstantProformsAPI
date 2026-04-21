namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines email delivery operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email with an optional PDF attachment.
    /// </summary>
    /// <param name="to">The destination email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="body">The email body.</param>
    /// <param name="attachmentFileName">The optional attachment file name.</param>
    /// <param name="attachmentContent">The optional attachment content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendAsync(
        string to,
        string subject,
        string body,
        string? attachmentFileName,
        byte[]? attachmentContent,
        CancellationToken cancellationToken);
}