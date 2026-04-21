using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Features.Proforms.GetProformById;

namespace InstantProforms.Application.Features.Proforms.DownloadProformPdf;

/// <summary>
/// Handles generation of a proform PDF.
/// </summary>
public sealed class DownloadProformPdfQueryHandler
    : IRequestHandler<DownloadProformPdfQuery, DownloadProformPdfResponse>
{
    private readonly ISender _sender;
    private readonly IProformPdfService _proformPdfService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadProformPdfQueryHandler"/> class.
    /// </summary>
    /// <param name="sender">The sender used to retrieve proform details.</param>
    /// <param name="proformPdfService">The PDF generation service.</param>
    public DownloadProformPdfQueryHandler(
        ISender sender,
        IProformPdfService proformPdfService)
    {
        _sender = sender;
        _proformPdfService = proformPdfService;
    }

    /// <inheritdoc />
    public async Task<DownloadProformPdfResponse> Handle(
        DownloadProformPdfQuery request,
        CancellationToken cancellationToken)
    {
        var proform = await _sender.Send(
            new GetProformByIdQuery(request.ProformId),
            cancellationToken);

        var content = _proformPdfService.Generate(proform);

        return new DownloadProformPdfResponse(
            content,
            $"{proform.Number}.pdf",
            "application/pdf");
    }
}