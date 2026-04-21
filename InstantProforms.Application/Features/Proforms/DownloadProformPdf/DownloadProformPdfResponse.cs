namespace InstantProforms.Application.Features.Proforms.DownloadProformPdf;

/// <summary>
/// Represents the generated PDF document result.
/// </summary>
public sealed record DownloadProformPdfResponse(
    byte[] Content,
    string FileName,
    string ContentType);