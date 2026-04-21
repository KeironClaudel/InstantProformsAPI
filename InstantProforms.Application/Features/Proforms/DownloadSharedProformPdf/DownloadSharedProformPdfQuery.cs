using MediatR;

namespace InstantProforms.Application.Features.Proforms.DownloadSharedProformPdf;

/// <summary>
/// Represents a request to download a shared proform PDF using a temporary token.
/// </summary>
public sealed record DownloadSharedProformPdfQuery(string Token) : IRequest<DownloadSharedProformPdfResponse>;