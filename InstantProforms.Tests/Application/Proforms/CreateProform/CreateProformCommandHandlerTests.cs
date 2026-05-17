using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Features.Proforms.CreateProform;
using InstantProforms.Domain.Entities;
using InstantProforms.Domain.Enums;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Application.Proforms.CreateProform;

public sealed class CreateProformCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUsingSavedClient_AppliesFallbackDataAndDefaults()
    {
        var companyId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var capturedProform = default(Proform);

        var selectedClient = new Client
        {
            Id = clientId,
            CompanyId = companyId,
            Name = "Cliente Guardado",
            Email = "guardado@test.com",
            Phone = "2222-3333",
            IdentificationType = ClientIdentificationType.PhysicalId,
            IdentificationNumber = "1-2345-6789",
            IsActive = true
        };

        var settings = new CompanySettings
        {
            CompanyId = companyId,
            TaxPercentage = 13m
        };

        var clientsRepository = new Mock<IClientRepository>();
        clientsRepository
            .Setup(x => x.GetByIdAsync(clientId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(selectedClient);

        var companySettingsRepository = new Mock<ICompanySettingsRepository>();
        companySettingsRepository
            .Setup(x => x.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(settings);

        var proformsRepository = new Mock<IProformRepository>();
        proformsRepository
            .Setup(x => x.GetLatestByCompanyAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Proform?)null);
        proformsRepository
            .Setup(x => x.AddAsync(It.IsAny<Proform>(), It.IsAny<CancellationToken>()))
            .Callback<Proform, CancellationToken>((proform, _) => capturedProform = proform)
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.Clients).Returns(clientsRepository.Object);
        unitOfWork.SetupGet(x => x.CompanySettings).Returns(companySettingsRepository.Object);
        unitOfWork.SetupGet(x => x.Proforms).Returns(proformsRepository.Object);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(x => x.CompanyId).Returns(companyId);

        var command = new CreateProformCommand(
            ClientId: clientId,
            ClientName: string.Empty,
            ClientEmail: null,
            ClientPhone: null,
            Notes: "  Ubicacion legacy  ",
            Location: null,
            InternalNotes: "  Nota interna  ",
            ClientIdentificationType: null,
            ClientIdentificationNumber: null,
            Currency: null,
            ServiceDescription: "  Descripcion del servicio  ",
            ScopeOfWork: "  Alcance detallado  ",
            ServiceConditions: null,
            PaymentConditions: null,
            Items: new[]
            {
                new CreateProformItemModel("Diseno y desarrollo", 2m, 150m)
            });

        var secretProtector = new Mock<ISecretProtector>();
        secretProtector
            .Setup(x => x.Protect(It.IsAny<string>()))
            .Returns<string>(value => $"protected::{value}");

        var handler = new CreateProformCommandHandler(
            unitOfWork.Object,
            currentUserService.Object,
            secretProtector.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(capturedProform);
        Assert.Equal(clientId, capturedProform!.ClientId);
        Assert.Equal("Cliente Guardado", capturedProform.ClientName);
        Assert.Equal("guardado@test.com", capturedProform.ClientEmail);
        Assert.Equal("2222-3333", capturedProform.ClientPhone);
        Assert.Equal(ClientIdentificationType.PhysicalId, capturedProform.ClientIdentificationType);
        Assert.Equal("1-2345-6789", capturedProform.ClientIdentificationNumber);
        Assert.Equal("protected::1-2345-6789", capturedProform.ClientIdentificationNumberEncrypted);
        Assert.Equal("Ubicacion legacy", capturedProform.Location);
        Assert.Equal("Nota interna", capturedProform.InternalNotes);
        Assert.Equal("Descripcion del servicio", capturedProform.ServiceDescription);
        Assert.Equal("Alcance detallado", capturedProform.ScopeOfWork);
        Assert.Equal(ProformCurrency.Colones, capturedProform.Currency);
        Assert.Equal(ProformNumberGenerator.GenerateNextNumber(null, DateTime.UtcNow.Year), result.Number);
        Assert.Equal("Colones", result.Currency);
        Assert.Equal(300m, result.Subtotal);
        Assert.Equal(39m, result.TaxAmount);
        Assert.Equal(339m, result.Total);
    }
}
