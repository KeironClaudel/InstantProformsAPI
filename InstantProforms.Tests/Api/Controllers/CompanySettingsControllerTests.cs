using InstantProforms.Api.Controllers;
using InstantProforms.Application.Features.CompanyConfig.GetCompanySettings;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Api.Controllers;

public sealed class CompanySettingsControllerTests
{
    [Fact]
    public async Task Get_ReturnsPublicLogoUrlWithoutRewritingItToTheBackendHost()
    {
        const string relativeLogoPath = "uploads/company-logos/5e44d6cc-15b2-4c4d-af21-b361d3325401/logo.png";
        const string publicLogoUrl = "https://demo.supabase.co/storage/v1/object/public/company-assets/uploads/company-logos/5e44d6cc-15b2-4c4d-af21-b361d3325401/logo.png";

        var response = new GetCompanySettingsResponse(
            DisplayName: "Instant Proforms",
            LegalName: "Instant Proforms LLC",
            Website: "https://instantproforms.com",
            Phone: "+506 8888-8888",
            Email: "hello@instantproforms.com",
            Address: "San Jose, Costa Rica",
            TermsAndConditions: "Payment due on receipt.",
            LogoFileName: relativeLogoPath,
            LogoUrl: publicLogoUrl,
            PrimaryColor: "#111827",
            SecondaryColor: "#2563eb",
            AccentColor: "#22c55e",
            ProformPrefix: "PRO",
            TaxPercentage: 13m,
            CurrencySymbol: "$",
            TaxLabel: "IVA");

        var sender = new Mock<ISender>(MockBehavior.Strict);
        sender
            .Setup(x => x.Send(It.IsAny<IRequest<GetCompanySettingsResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new CompanySettingsController(sender.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        controller.Request.Scheme = "https";
        controller.Request.Host = new HostString("instantproforms-api.onrender.com");

        var actionResult = await controller.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var body = Assert.IsType<GetCompanySettingsResponse>(okResult.Value);

        Assert.Equal(relativeLogoPath, body.LogoFileName);
        Assert.Equal(publicLogoUrl, body.LogoUrl);
        Assert.False(
            body.LogoUrl?.StartsWith("https://instantproforms-api.onrender.com", StringComparison.Ordinal) ?? false);
    }
}
