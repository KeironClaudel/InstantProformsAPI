using MediatR;

namespace InstantProforms.Application.Features.Proforms.DownloadProformPdf;

/// <summary>
/// Represents a request to generate a PDF for a proform.
/// </summary>
public sealed record DownloadProformPdfQuery(Guid ProformId) : IRequest<DownloadProformPdfResponse>;