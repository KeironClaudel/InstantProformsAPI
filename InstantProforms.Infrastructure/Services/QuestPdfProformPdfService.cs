using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Features.Proforms.Common;
using InstantProforms.Infrastructure.Services.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Generates proform PDF documents using QuestPDF.
/// </summary>
public sealed class QuestPdfProformPdfService : IProformPdfService
{
    /// <inheritdoc />
    public byte[] Generate(ProformPdfModel model)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var logoPath = model.LogoFileName is null
            ? Path.Combine(AppContext.BaseDirectory, "Assets", "Logo2020.png")
            : Path.Combine(AppContext.BaseDirectory, "Assets", "CompanyLogos", model.LogoFileName);

        var document = new ProformPdfDocument(model, logoPath);

        return document.GeneratePdf();
    }
}