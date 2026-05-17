using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Application.Features.CompanyConfig.GetCompanySettings;
using InstantProforms.Domain.Entities;
using InstantProforms.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Application.CompanyConfig;

public sealed class GetCompanySettingsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsDecryptedTenantResendFieldsWithoutExposingApiKey()
    {
        var companyId = Guid.NewGuid();
        var protector = CreateProtector();
        var settings = new CompanySettings
        {
            CompanyId = companyId,
            DisplayName = "Eco",
            TaxLabel = "IVA",
            CurrencySymbol = "₡",
            ResendApiKeyEncrypted = protector.Protect("re_secret"),
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

        var currentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(x => x.CompanyId).Returns(companyId);

        var fileStorageService = new Mock<IFileStorageService>(MockBehavior.Strict);
        fileStorageService.Setup(x => x.GetPublicUrl(It.IsAny<string?>())).Returns((string?)null);

        var handler = new GetCompanySettingsQueryHandler(
            unitOfWork.Object,
            currentUserService.Object,
            fileStorageService.Object,
            protector);

        var response = await handler.Handle(new GetCompanySettingsQuery(), CancellationToken.None);

        Assert.True(response.HasResendApiKeyConfigured);
        Assert.True(response.IsResendEmailDeliveryConfigured);
        Assert.Equal("noreply@eco.test", response.ResendSenderEmail);
        Assert.Equal("Eco Team", response.ResendSenderName);
        Assert.Equal("support@eco.test", response.ResendReplyToEmail);
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
