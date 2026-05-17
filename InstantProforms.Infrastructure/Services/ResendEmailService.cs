using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using InstantProforms.Application.Common.Exceptions;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Sends emails using the Resend API.
/// </summary>
public sealed class ResendEmailService : IEmailService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    private readonly HttpClient _httpClient;
    private readonly ICompanySettingsRepository _companySettingsRepository;
    private readonly ISecretProtector _secretProtector;
    private readonly ResendSettings _settings;
    private readonly ILogger<ResendEmailService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResendEmailService"/> class.
    /// </summary>
    public ResendEmailService(
        HttpClient httpClient,
        ICompanySettingsRepository companySettingsRepository,
        ISecretProtector secretProtector,
        ILogger<ResendEmailService> logger,
        IOptions<ResendSettings> settings)
    {
        _httpClient = httpClient;
        _companySettingsRepository = companySettingsRepository;
        _secretProtector = secretProtector;
        _logger = logger;
        _settings = settings.Value;

        _httpClient.BaseAddress ??= new Uri(
            string.IsNullOrWhiteSpace(_settings.BaseUrl)
                ? "https://api.resend.com/"
                : _settings.BaseUrl.Trim());
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <inheritdoc />
    public async Task SendAsync(
        Guid companyId,
        string to,
        string subject,
        string body,
        string? attachmentFileName,
        byte[]? attachmentContent,
        CancellationToken cancellationToken)
    {
        var emailConfiguration = await ResolveEmailConfigurationAsync(companyId, cancellationToken);

        var payload = new ResendEmailRequest(
            From: FormatFromAddress(emailConfiguration),
            To: [to],
            Subject: subject,
            Html: body,
            ReplyTo: BuildReplyTo(emailConfiguration),
            Attachments: BuildAttachments(attachmentFileName, attachmentContent));

        using var request = new HttpRequestMessage(HttpMethod.Post, "emails")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload, SerializerOptions),
                Encoding.UTF8,
                "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", emailConfiguration.ApiKey);

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogError(
            "Resend email delivery failed. StatusCode: {StatusCode}. ResponseBody: {ResponseBody}",
            (int)response.StatusCode,
            responseBody);

        throw new ExternalServiceException(
            "Email delivery failed. Please try again later or verify your company email settings.");
    }

    private async Task<CompanyEmailConfiguration> ResolveEmailConfigurationAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var settings = await _companySettingsRepository.GetByCompanyIdAsync(companyId, cancellationToken);

        if (settings is null)
        {
            throw new ConfigurationException("Email delivery is not configured for this company.");
        }

        var apiKey = UnprotectRequired(settings.ResendApiKeyEncrypted, "Resend API key");
        var senderEmail = UnprotectRequired(settings.ResendSenderEmailEncrypted, "Resend sender email");
        var senderName = UnprotectOptional(settings.ResendSenderNameEncrypted);
        var replyToEmail = UnprotectOptional(settings.ResendReplyToEmailEncrypted);

        return new CompanyEmailConfiguration(apiKey, senderEmail, senderName, replyToEmail);
    }

    private static string FormatFromAddress(CompanyEmailConfiguration configuration)
    {
        return string.IsNullOrWhiteSpace(configuration.SenderName)
            ? configuration.SenderEmail.Trim()
            : $"{configuration.SenderName.Trim()} <{configuration.SenderEmail.Trim()}>";
    }

    private static IReadOnlyList<ResendAttachmentRequest>? BuildAttachments(
        string? attachmentFileName,
        byte[]? attachmentContent)
    {
        if (string.IsNullOrWhiteSpace(attachmentFileName)
            || attachmentContent is null
            || attachmentContent.Length == 0)
        {
            return null;
        }

        return
        [
            new ResendAttachmentRequest(
                attachmentFileName.Trim(),
                Convert.ToBase64String(attachmentContent))
        ];
    }

    private static IReadOnlyList<string>? BuildReplyTo(CompanyEmailConfiguration configuration)
    {
        return string.IsNullOrWhiteSpace(configuration.ReplyToEmail)
            ? null
            : [configuration.ReplyToEmail.Trim()];
    }

    private string UnprotectRequired(string? encryptedValue, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(encryptedValue))
        {
            throw new ConfigurationException(
                $"{fieldName} is not configured for this company. Update company settings before sending emails.");
        }

        try
        {
            return _secretProtector.Unprotect(encryptedValue);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception, "Failed to decrypt company email configuration field {FieldName}.", fieldName);
            throw new ConfigurationException(
                "Email delivery configuration is invalid. Please update your company email settings.");
        }
    }

    private string? UnprotectOptional(string? encryptedValue)
    {
        if (string.IsNullOrWhiteSpace(encryptedValue))
        {
            return null;
        }

        try
        {
            return _secretProtector.Unprotect(encryptedValue);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception, "Failed to decrypt an optional company email configuration field.");
            throw new ConfigurationException(
                "Email delivery configuration is invalid. Please update your company email settings.");
        }
    }

    private sealed record ResendEmailRequest(
        string From,
        IReadOnlyList<string> To,
        string Subject,
        string Html,
        [property: JsonPropertyName("reply_to")] IReadOnlyList<string>? ReplyTo,
        IReadOnlyList<ResendAttachmentRequest>? Attachments);

    private sealed record ResendAttachmentRequest(
        string Filename,
        string Content);

    private sealed record CompanyEmailConfiguration(
        string ApiKey,
        string SenderEmail,
        string? SenderName,
        string? ReplyToEmail);
}
