using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Features.Proforms.Common;
using InstantProforms.Infrastructure.Services.Pdf;
using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Generates proform PDF documents using QuestPDF.
/// </summary>
public sealed class QuestPdfProformPdfService : IProformPdfService
{
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestPdfProformPdfService"/> class.
    /// </summary>
    /// <param name="environment">The web host environment.</param>
    public QuestPdfProformPdfService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <inheritdoc />
    public byte[] Generate(ProformPdfModel model)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var logoPath = string.IsNullOrWhiteSpace(model.LogoFileName)
            ? Path.Combine(_environment.ContentRootPath, "Assets", "default-logo.png")
            : Path.Combine(_environment.ContentRootPath, "wwwroot", model.LogoFileName.Replace("/", Path.DirectorySeparatorChar.ToString()));

        var document = new ProformPdfDocument(model, logoPath);

        return document.GeneratePdf();
    }
}