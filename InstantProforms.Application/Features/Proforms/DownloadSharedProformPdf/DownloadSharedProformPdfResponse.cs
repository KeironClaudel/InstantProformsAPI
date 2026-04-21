namespace InstantProforms.Application.Features.Proforms.DownloadSharedProformPdf;

/// <summary>
/// Represents the generated shared PDF result.
/// </summary>
public sealed record DownloadSharedProformPdfResponse(
    byte[] Content,
    string FileName,
    string ContentType);