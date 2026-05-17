using System.Net;
using System.Net.Http;
using System.Text.Json;
using InstantProforms.Application.Common.Exceptions;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Domain.Entities;
using InstantProforms.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Infrastructure.Services;

public sealed class ResendEmailServiceTests
{
    private static readonly IOptions<SecretProtectionSettings> SecretProtectionOptions = Options.Create(
        new SecretProtectionSettings
        {
            MasterKey = Convert.ToBase64String(new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8,
                9, 10, 11, 12, 13, 14, 15, 16,
                17, 18, 19, 20, 21, 22, 23, 24,
                25, 26, 27, 28, 29, 30, 31, 32
            })
        });

    [Fact]
    public async Task SendAsync_WithoutAttachment_SendsExpectedPayload()
    {
        var handler = new CapturingHandler();
        var client = new HttpClient(handler);
        var secretProtector = new AesGcmSecretProtector(SecretProtectionOptions);
        var service = new ResendEmailService(
            client,
            BuildCompanySettingsRepository(secretProtector),
            secretProtector,
            NullLogger<ResendEmailService>.Instance,
            Options.Create(new ResendSettings()));

        await service.SendAsync(
            Guid.NewGuid(),
            "client@example.com",
            "Hello",
            "<p>Body</p>",
            attachmentFileName: null,
            attachmentContent: null,
            CancellationToken.None);

        Assert.NotNull(handler.Request);
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.Equal("https://api.resend.com/emails", handler.Request.RequestUri!.ToString());
        Assert.Equal("Bearer", handler.Request.Headers.Authorization?.Scheme);
        Assert.Equal("re_test_key", handler.Request.Headers.Authorization?.Parameter);

        using var payload = JsonDocument.Parse(handler.RequestBody!);
        Assert.Equal("InstantProforms <hello@example.com>", payload.RootElement.GetProperty("from").GetString());
        Assert.Equal("client@example.com", payload.RootElement.GetProperty("to")[0].GetString());
        Assert.Equal("Hello", payload.RootElement.GetProperty("subject").GetString());
        Assert.Equal("<p>Body</p>", payload.RootElement.GetProperty("html").GetString());
        Assert.False(payload.RootElement.TryGetProperty("attachments", out _));
    }

    [Fact]
    public async Task SendAsync_WithAttachment_EncodesAttachmentAsBase64()
    {
        var handler = new CapturingHandler();
        var client = new HttpClient(handler);
        var secretProtector = new AesGcmSecretProtector(SecretProtectionOptions);
        var service = new ResendEmailService(
            client,
            BuildCompanySettingsRepository(secretProtector),
            secretProtector,
            NullLogger<ResendEmailService>.Instance,
            Options.Create(new ResendSettings()));

        var attachmentBytes = new byte[] { 1, 2, 3, 4 };

        await service.SendAsync(
            Guid.NewGuid(),
            "client@example.com",
            "Attachment",
            "<p>Attached</p>",
            "quote.pdf",
            attachmentBytes,
            CancellationToken.None);

        using var payload = JsonDocument.Parse(handler.RequestBody!);
        var attachment = payload.RootElement.GetProperty("attachments")[0];

        Assert.Equal("quote.pdf", attachment.GetProperty("filename").GetString());
        Assert.Equal(Convert.ToBase64String(attachmentBytes), attachment.GetProperty("content").GetString());
    }

    [Fact]
    public async Task SendAsync_WithReplyTo_IncludesReplyToField()
    {
        var handler = new CapturingHandler();
        var client = new HttpClient(handler);
        var secretProtector = new AesGcmSecretProtector(SecretProtectionOptions);
        var service = new ResendEmailService(
            client,
            BuildCompanySettingsRepository(secretProtector, replyToEmail: "support@example.com"),
            secretProtector,
            NullLogger<ResendEmailService>.Instance,
            Options.Create(new ResendSettings()));

        await service.SendAsync(
            Guid.NewGuid(),
            "client@example.com",
            "Reply",
            "<p>Body</p>",
            attachmentFileName: null,
            attachmentContent: null,
            CancellationToken.None);

        using var payload = JsonDocument.Parse(handler.RequestBody!);
        Assert.Equal("support@example.com", payload.RootElement.GetProperty("reply_to")[0].GetString());
    }

    [Fact]
    public async Task SendAsync_WhenProviderFails_ThrowsSafeExternalServiceException()
    {
        var handler = new CapturingHandler(HttpStatusCode.BadRequest, """{"message":"provider details"}""");
        var client = new HttpClient(handler);
        var secretProtector = new AesGcmSecretProtector(SecretProtectionOptions);
        var service = new ResendEmailService(
            client,
            BuildCompanySettingsRepository(secretProtector),
            secretProtector,
            NullLogger<ResendEmailService>.Instance,
            Options.Create(new ResendSettings()));

        var exception = await Assert.ThrowsAsync<ExternalServiceException>(() => service.SendAsync(
            Guid.NewGuid(),
            "client@example.com",
            "Failure",
            "<p>Body</p>",
            attachmentFileName: null,
            attachmentContent: null,
            CancellationToken.None));

        Assert.DoesNotContain("provider details", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SendAsync_WithoutCompanyApiKey_ThrowsClearConfigurationError()
    {
        var handler = new CapturingHandler();
        var client = new HttpClient(handler);
        var secretProtector = new AesGcmSecretProtector(SecretProtectionOptions);
        var repository = new Mock<ICompanySettingsRepository>(MockBehavior.Strict);
        repository
            .Setup(x => x.GetByCompanyIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanySettings
            {
                ResendSenderEmailEncrypted = secretProtector.Protect("hello@example.com")
            });

        var service = new ResendEmailService(
            client,
            repository.Object,
            secretProtector,
            NullLogger<ResendEmailService>.Instance,
            Options.Create(new ResendSettings()));

        var exception = await Assert.ThrowsAsync<ConfigurationException>(() => service.SendAsync(
            Guid.NewGuid(),
            "client@example.com",
            "Reply",
            "<p>Body</p>",
            attachmentFileName: null,
            attachmentContent: null,
            CancellationToken.None));

        Assert.Contains("Resend API key is not configured", exception.Message);
    }

    private static ICompanySettingsRepository BuildCompanySettingsRepository(
        AesGcmSecretProtector secretProtector,
        string? replyToEmail = null)
    {
        var repository = new Mock<ICompanySettingsRepository>(MockBehavior.Strict);
        repository
            .Setup(x => x.GetByCompanyIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanySettings
            {
                ResendApiKeyEncrypted = secretProtector.Protect("re_test_key"),
                ResendSenderEmailEncrypted = secretProtector.Protect("hello@example.com"),
                ResendSenderNameEncrypted = secretProtector.Protect("InstantProforms"),
                ResendReplyToEmailEncrypted = string.IsNullOrWhiteSpace(replyToEmail)
                    ? null
                    : secretProtector.Protect(replyToEmail)
            });

        return repository.Object;
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseBody;

        public CapturingHandler(HttpStatusCode statusCode = HttpStatusCode.OK, string responseBody = """{"id":"email_123"}""")
        {
            _statusCode = statusCode;
            _responseBody = responseBody;
        }

        public HttpRequestMessage? Request { get; private set; }
        public string? RequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            RequestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseBody)
            };
        }
    }
}
