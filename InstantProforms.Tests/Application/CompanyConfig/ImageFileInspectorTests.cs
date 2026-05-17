using InstantProforms.Application.Common.Files;
using InstantProforms.Application.Features.Auth.RegisterCompany;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace InstantProforms.Tests.Application.CompanyConfig;

public sealed class ImageFileInspectorTests
{
    [Fact]
    public void IsSupportedImage_ReturnsFalse_ForHtmlDisguisedAsPng()
    {
        var file = CreateFormFile(
            "<script>alert('xss')</script>"u8.ToArray(),
            "logo.png",
            "image/png");

        var isSupported = ImageFileInspector.IsSupportedImage(file);

        Assert.False(isSupported);
    }

    [Fact]
    public void RegisterCompanyValidator_RejectsFileWhoseBytesDoNotMatchExtension()
    {
        var validator = new RegisterCompanyCommandValidator();
        var command = new RegisterCompanyCommand(
            CompanyName: "Instant Proforms",
            CompanySlug: "instant-proforms",
            CompanyEmail: "info@example.com",
            CompanyPhone: "555-0100",
            CompanyAddress: "San Jose",
            CompanyWebsite: "https://example.com",
            DisplayName: "Instant",
            LegalName: "Instant Proforms S.A.",
            TermsAndConditions: "Net 30",
            PrimaryColor: "#111111",
            SecondaryColor: "#222222",
            AccentColor: "#333333",
            ProformPrefix: "IP",
            CurrencySymbol: "$",
            TaxLabel: "IVA",
            TaxPercentage: 13,
            LogoFile: CreateFormFile("<html>owned</html>"u8.ToArray(), "logo.png", "image/png"),
            OwnerFullName: "Owner User",
            OwnerEmail: "owner@example.com",
            Password: "ValidPass1");

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "LogoFile");
    }

    private static IFormFile CreateFormFile(byte[] content, string fileName, string contentType)
    {
        var stream = new MemoryStream(content);

        return new FormFile(stream, 0, content.Length, "logoFile", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
