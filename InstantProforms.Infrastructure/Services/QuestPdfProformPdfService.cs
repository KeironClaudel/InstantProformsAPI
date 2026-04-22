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

        var basePath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        var logoPath = Path.Combine(basePath, "Assets", "default-logo.png"); // fallback

        if (!string.IsNullOrWhiteSpace(model.LogoFileName))
        {
            var candidatePath = Path.Combine(
                basePath,
                model.LogoFileName.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(candidatePath))
            {
                logoPath = candidatePath;
            }
        }

        var document = new ProformPdfDocument(model, logoPath);

        return document.GeneratePdf();
    }
}