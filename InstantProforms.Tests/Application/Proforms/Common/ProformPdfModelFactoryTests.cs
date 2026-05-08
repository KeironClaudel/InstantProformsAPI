using InstantProforms.Application.Features.Proforms.Common;
using InstantProforms.Domain.Entities;
using InstantProforms.Domain.Enums;
using Xunit;

namespace InstantProforms.Tests.Application.Proforms.Common;

public sealed class ProformPdfModelFactoryTests
{
    [Fact]
    public void Create_MapsCurrencyAndEditorialSections()
    {
        var proform = new Proform
        {
            Id = Guid.NewGuid(),
            Number = "C2026200",
            Status = ProformStatus.Sent,
            Currency = ProformCurrency.Dollars,
            ClientName = "Cliente Demo",
            ClientEmail = "cliente@test.com",
            ClientPhone = "1111-2222",
            ClientIdentificationType = ClientIdentificationType.LegalEntityId,
            ClientIdentificationNumber = "3-101-999999",
            IssuedAtUtc = new DateTime(2026, 5, 7, 0, 0, 0, DateTimeKind.Utc),
            Location = "San Jose",
            InternalNotes = "Nota interna",
            ServiceDescription = "Descripcion larga",
            ScopeOfWork = "Alcance detallado",
            ServiceConditions = "Condiciones del servicio",
            PaymentConditions = "Condiciones de pago",
            Subtotal = 100m,
            TaxPercentage = 13m,
            TaxAmount = 13m,
            Total = 113m,
            Items =
            {
                new ProformItem
                {
                    Id = Guid.NewGuid(),
                    Description = "Diseno",
                    Quantity = 1,
                    UnitPrice = 100m,
                    Total = 100m,
                    SortOrder = 1
                }
            }
        };

        var settings = new CompanySettings
        {
            DisplayName = "Instant Proforms",
            TaxLabel = "IVA (13%)",
            TermsAndConditions = "Condicion general",
            PrimaryColor = "#123456"
        };

        var result = ProformPdfModelFactory.Create(proform, settings);

        Assert.Equal("$", result.CurrencySymbol);
        Assert.Equal("LegalEntityId", result.ClientIdentificationType);
        Assert.Equal("3-101-999999", result.ClientIdentificationNumber);
        Assert.Equal("Descripcion larga", result.ServiceDescription);
        Assert.Equal("Alcance detallado", result.ScopeOfWork);
        Assert.Equal("Condiciones del servicio", result.ServiceConditions);
        Assert.Equal("Condiciones de pago", result.PaymentConditions);
        Assert.Equal("IVA (13%)", result.TaxLabel);
        Assert.Single(result.Items);
    }
}
