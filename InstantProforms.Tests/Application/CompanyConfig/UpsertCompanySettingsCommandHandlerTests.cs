using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Application.Features.CompanyConfig.UpsertCompanySettings;
using InstantProforms.Domain.Entities;
using InstantProforms.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Application.CompanyConfig;

public sealed class UpsertCompanySettingsCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNewApiKey_EncryptsAndPersistsTenantEmailConfiguration()
    {
        var companyId = Guid.NewGuid();
        var settings = new CompanySettings
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            DisplayName = "Eco"
        };

        var companySettingsRepository = new Mock<ICompanySettingsRepository>(MockBehavior.Strict);
        companySettingsRepository
            .Setup(x => x.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(settings);

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.CompanySettings).Returns(companySettingsRepository.Object);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var currentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(x => x.CompanyId).Returns(companyId);

        var protector = CreateProtector();
        var handler = new UpsertCompanySettingsCommandHandler(
            unitOfWork.Object,
            currentUserService.Object,
            protector);

        var command = new UpsertCompanySettingsCommand(
            "Eco",
            null,
            null,
            null,
            "hello@eco.test",
            null,
            null,
            null,
            "#000000",
            "#ffffff",
            13m,
            "#123456",
            "PRO",
            "₡",
            "IVA",
            "re_live_key",
            false,
            "noreply@eco.test",
            "Eco Team",
            "support@eco.test");

        await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(settings.ResendApiKeyEncrypted);
        Assert.Equal("re_live_key", protector.Unprotect(settings.ResendApiKeyEncrypted!));
        Assert.Equal("noreply@eco.test", protector.Unprotect(settings.ResendSenderEmailEncrypted!));
        Assert.Equal("Eco Team", protector.Unprotect(settings.ResendSenderNameEncrypted!));
        Assert.Equal("support@eco.test", protector.Unprotect(settings.ResendReplyToEmailEncrypted!));
    }

    [Fact]
    public async Task Handle_WithClearResendApiKey_RemovesStoredApiKeyButKeepsOtherFields()
    {
        var companyId = Guid.NewGuid();
        var protector = CreateProtector();
        var settings = new CompanySettings
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            DisplayName = "Eco",
            ResendApiKeyEncrypted = protector.Protect("re_existing"),
            ResendSenderEmailEncrypted = protector.Protect("noreply@eco.test"),
            ResendSenderNameEncrypted = protector.Protect("Eco Team"),
            ResendReplyToEmailEncrypted = protector.Protect("support@eco.test")
        };

        var companySettingsRepository = new Mock<ICompanySettingsRepository>(MockBehavior.Strict);
        companySettingsRepository
            .Setup(x => x.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(settings);

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.CompanySettings).Returns(companySettingsRepository.Object);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var currentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(x => x.CompanyId).Returns(companyId);

        var handler = new UpsertCompanySettingsCommandHandler(
            unitOfWork.Object,
            currentUserService.Object,
            protector);

        var command = new UpsertCompanySettingsCommand(
            "Eco",
            null,
            null,
            null,
            "hello@eco.test",
            null,
            null,
            null,
            "#000000",
            "#ffffff",
            13m,
            "#123456",
            "PRO",
            "₡",
            "IVA",
            null,
            true,
            "noreply@eco.test",
            "Eco Team",
            "support@eco.test");

        await handler.Handle(command, CancellationToken.None);

        Assert.Null(settings.ResendApiKeyEncrypted);
        Assert.Equal("noreply@eco.test", protector.Unprotect(settings.ResendSenderEmailEncrypted!));
    }

    private static AesGcmSecretProtector CreateProtector()
    {
        return new AesGcmSecretProtector(Options.Create(new SecretProtectionSettings
        {
            MasterKey = Convert.ToBase64String(new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8,
                9, 10, 11, 12, 13, 14, 15, 16,
                17, 18, 19, 20, 21, 22, 23, 24,
                25, 26, 27, 28, 29, 30, 31, 32
            })
        }));
    }
}
