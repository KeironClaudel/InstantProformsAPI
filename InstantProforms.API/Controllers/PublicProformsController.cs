using MediatR;
using Microsoft.AspNetCore.Mvc;
using InstantProforms.Application.Features.Proforms.DownloadSharedProformPdf;

namespace InstantProforms.Api.Controllers;

/// <summary>
/// Provides public endpoints for shared proform downloads.
/// </summary>
[ApiController]
[Route("api/public/proforms")]
public sealed class PublicProformsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicProformsController"/> class.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public PublicProformsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Downloads a shared proform PDF using a temporary token.
    /// </summary>
    /// <param name="token">The raw share token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated PDF file.</returns>
    [HttpGet("download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Download(
        [FromQuery] string token,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            new DownloadSharedProformPdfQuery(token),
            cancellationToken);

        return File(response.Content, response.ContentType, response.FileName);
    }
}