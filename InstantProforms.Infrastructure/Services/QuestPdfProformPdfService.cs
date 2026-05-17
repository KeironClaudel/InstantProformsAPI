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
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestPdfProformPdfService"/> class.
    /// </summary>
    /// <param name="fileStorageService">The file storage service.</param>
    public QuestPdfProformPdfService(
        IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    /// <inheritdoc />
    public async Task<byte[]> GenerateAsync(ProformPdfModel model, CancellationToken cancellationToken)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var logoBytes = await _fileStorageService.GetBytesAsync(model.LogoFileName, cancellationToken);

        var document = new ProformPdfDocument(model, logoBytes);

        return document.GeneratePdf();
    }
}
