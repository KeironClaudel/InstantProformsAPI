using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Application.Features.Clients.CreateClient;
using InstantProforms.Domain.Entities;
using InstantProforms.Domain.Enums;
using InstantProforms.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Application.Clients;

public sealed class CreateClientCommandHandlerTests
{
    [Fact]
    public async Task Handle_EncryptsIdentificationNumberAndReturnsPlainValue()
    {
        var companyId = Guid.NewGuid();
        Client? capturedClient = null;

        var clientsRepository = new Mock<IClientRepository>(MockBehavior.Strict);
        clientsRepository
            .Setup(x => x.IsIdentificationInUseAsync(
                companyId,
                ClientIdentificationType.LegalEntityId,
                "3-101-999999",
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        clientsRepository
            .Setup(x => x.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
            .Callback<Client, CancellationToken>((client, _) => capturedClient = client)
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.Clients).Returns(clientsRepository.Object);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var currentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(x => x.CompanyId).Returns(companyId);

        var protector = CreateProtector();
        var fingerprintService = CreateFingerprintService();

        var handler = new CreateClientCommandHandler(
            currentUserService.Object,
            unitOfWork.Object,
            protector,
            fingerprintService);

        var response = await handler.Handle(new CreateClientCommand(
            "Cliente Demo",
            "demo@test.com",
            "2222-3333",
            ClientIdentificationType.LegalEntityId,
            "3-101-999999"), CancellationToken.None);

        Assert.NotNull(capturedClient);
        Assert.Equal("3-101-999999", capturedClient!.IdentificationNumber);
        Assert.NotNull(capturedClient.IdentificationNumberEncrypted);
        Assert.NotNull(capturedClient.IdentificationNumberHash);
        Assert.Equal("3-101-999999", protector.Unprotect(capturedClient.IdentificationNumberEncrypted!));
        Assert.Equal(
            fingerprintService.ComputeFingerprint("3-101-999999"),
            capturedClient.IdentificationNumberHash);
        Assert.Equal("3-101-999999", response.IdentificationNumber);
    }

    private static AesGcmSecretProtector CreateProtector()
    {
        return new AesGcmSecretProtector(CreateSecretProtectionOptions());
    }

    private static HmacSecretFingerprintService CreateFingerprintService()
    {
        return new HmacSecretFingerprintService(CreateSecretProtectionOptions());
    }

    private static IOptions<SecretProtectionSettings> CreateSecretProtectionOptions()
    {
        return Options.Create(new SecretProtectionSettings
        {
            MasterKey = Convert.ToBase64String(new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8,
                9, 10, 11, 12, 13, 14, 15, 16,
                17, 18, 19, 20, 21, 22, 23, 24,
                25, 26, 27, 28, 29, 30, 31, 32
            })
        });
    }
}
