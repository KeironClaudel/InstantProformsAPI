using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Sends emails using SMTP.
/// </summary>
public sealed class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpEmailService"/> class.
    /// </summary>
    /// <param name="smtpSettings">The SMTP settings.</param>
    public SmtpEmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    /// <inheritdoc />
    public async Task SendAsync(
        string to,
        string subject,
        string body,
        string? attachmentFileName,
        byte[]? attachmentContent,
        CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = body
        };

        if (!string.IsNullOrWhiteSpace(attachmentFileName) &&
            attachmentContent is not null &&
            attachmentContent.Length > 0)
        {
            builder.Attachments.Add(attachmentFileName, attachmentContent, ContentType.Parse("application/pdf"));
        }

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        var secureSocketOptions = _smtpSettings.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        await client.ConnectAsync(
            _smtpSettings.Host,
            _smtpSettings.Port,
            secureSocketOptions,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(_smtpSettings.Username))
        {
            await client.AuthenticateAsync(
                _smtpSettings.Username,
                _smtpSettings.Password,
                cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}