using InstantProforms.Application.Features.Proforms.Common;
using InstantProforms.Infrastructure.Services.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Xunit;

namespace InstantProforms.Tests.Infrastructure.Services.Pdf;

public sealed class ProformPdfDocumentTests
{
    [Fact]
    public void GeneratePdf_WhenEditorialSectionsExist_ReturnsDocumentBytes()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var model = new ProformPdfModel(
            DisplayName: "Instant Proforms",
            Website: "https://instantproforms.test",
            Phone: "2222-3333",
            Email: "hello@test.com",
            Address: "San Jose, Costa Rica",
            LogoFileName: null,
            PrimaryColor: "#123456",
            SecondaryColor: "#DDE7FF",
            AccentColor: "#F5F8FF",
            CurrencySymbol: "₡",
            TaxLabel: "IVA (13%)",
            TermsAndConditions: "- Validez de 30 dias",
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
            IssuedAtUtc: new DateTime(2026, 5, 7, 0, 0, 0, DateTimeKind.Utc),
            Location: "Escazu",
            InternalNotes: "Solo interno",
            ServiceDescription: "Primer parrafo.\n\n- Punto uno\n- Punto dos",
            ScopeOfWork: "Detalle del alcance.",
            ServiceConditions: "Condiciones del servicio.",
            PaymentConditions: "Condiciones de pago.",
            Subtotal: 100m,
            Total: 113m,
            Items: new[]
            {
                new ProformPdfItemModel(Guid.NewGuid(), "Diseno", 1m, 100m, 100m, 1)
            });

        var document = new ProformPdfDocument(model, logoBytes: null);

        var bytes = document.GeneratePdf();

        Assert.NotNull(bytes);
        Assert.NotEmpty(bytes);
    }
}
