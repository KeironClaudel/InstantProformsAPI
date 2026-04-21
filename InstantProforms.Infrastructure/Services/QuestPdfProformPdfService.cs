using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Features.Proforms.GetProformById;
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
    public byte[] Generate(GetProformByIdResponse proform)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Logo2020.png");
        var document = new ProformPdfDocument(proform, logoPath);

        return document.GeneratePdf();
    }
}