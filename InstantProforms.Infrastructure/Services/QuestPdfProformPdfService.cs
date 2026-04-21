using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Features.Proforms.GetProformById;
using InstantProforms.Infrastructure.Services.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Generates proform PDF documents using QuestPDF.
/// </summary>
public sealed class QuestPdfProformPdfService : IProformPdfService
{
    /// <inheritdoc />
    public byte[] Generate(GetProformByIdResponse data)
    {
        var document = new ProformPdfDocument(data);
        return document.GeneratePdf();
    }
}