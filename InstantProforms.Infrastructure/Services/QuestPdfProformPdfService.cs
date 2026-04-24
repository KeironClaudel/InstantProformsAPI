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
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestPdfProformPdfService"/> class.
    /// </summary>
    /// <param name="environment">The web host environment.</param>
    /// <param name="fileStorageService">The file storage service.</param>
    public QuestPdfProformPdfService(
        IWebHostEnvironment environment,
        IFileStorageService fileStorageService)
    {
        _environment = environment;
        _fileStorageService = fileStorageService;
    }

    /// <inheritdoc />
    public async Task<byte[]> GenerateAsync(ProformPdfModel model, CancellationToken cancellationToken)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var logoBytes = await _fileStorageService.GetBytesAsync(model.LogoFileName, cancellationToken)
            ?? TryGetDefaultLogoBytes();

        var document = new ProformPdfDocument(model, logoBytes);

        return document.GeneratePdf();
    }

    private byte[]? TryGetDefaultLogoBytes()
    {
        var defaultLogoPath = Path.Combine(_environment.ContentRootPath, "Assets", "default-logo.png");
        return File.Exists(defaultLogoPath)
            ? File.ReadAllBytes(defaultLogoPath)
            : null;
    }
}
