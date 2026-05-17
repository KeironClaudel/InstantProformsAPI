using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Features.Proforms.Common;
using InstantProforms.Infrastructure.Services;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Infrastructure.Services;

public sealed class QuestPdfProformPdfServiceTests
{
    [Fact]
    public async Task GenerateAsync_WhenLogoIsMissing_GeneratesPdfWithoutRequiringFallbackLogo()
    {
        var fileStorageService = new Mock<IFileStorageService>();
        fileStorageService
            .Setup(service => service.GetBytesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var service = new QuestPdfProformPdfService(fileStorageService.Object);

        var model = new ProformPdfModel(
            DisplayName: "Servicios Profesionales",
            Website: null,
            Phone: "2222-3333",
            Email: "hello@test.com",
            Address: "San Jose, Costa Rica",
            LogoFileName: null,
            PrimaryColor: "#123456",
            SecondaryColor: "#DDE7FF",
            AccentColor: "#F5F8FF",
            CurrencySymbol: "₡",
            TaxLabel: "IVA (13%)",
            TermsAndConditions: null,
            ProformId: Guid.NewGuid(),
            Number: "C2026200",
            Status: "Draft",
            ClientName: "Cliente Demo",
            ClientEmail: "cliente@test.com",
            ClientPhone: "1111-2222",
            ClientIdentificationType: "PhysicalId",
            ClientIdentificationNumber: "1-2345-6789",
            TaxPercentage: 13m,
            TaxAmount: 13m,
            IssuedAtUtc: new DateTime(2026, 5, 17, 0, 0, 0, DateTimeKind.Utc),
            Location: "Escazu",
            InternalNotes: null,
            ServiceDescription: "Servicio profesional sin logo registrado.",
            ScopeOfWork: null,
            ServiceConditions: null,
            PaymentConditions: null,
            Subtotal: 100m,
            Total: 113m,
            Items: new[]
            {
                new ProformPdfItemModel(Guid.NewGuid(), "Diseno", 1m, 100m, 100m, 1)
            });

        var bytes = await service.GenerateAsync(model, CancellationToken.None);

        Assert.NotNull(bytes);
        Assert.NotEmpty(bytes);
    }
}
