using InstantProforms.Application.Features.Proforms.DownloadSharedProformPdf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstantProforms.Api.Controllers;

/// <summary>
/// Provides public endpoints for shared proform downloads.
/// </summary>
[ApiController]
[AllowAnonymous]
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
    /// Downloads a shared proform PDF using a temporary token encoded in the path.
    /// </summary>
    /// <param name="token">The raw share token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated PDF file.</returns>
    [HttpGet("download/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadByPath(
        string token,
        CancellationToken cancellationToken)
    {
        return await DownloadInternal(token, cancellationToken);
    }

    /// <summary>
    /// Downloads a shared proform PDF using a temporary token passed in the query string.
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
        return await DownloadInternal(token, cancellationToken);
    }

    private async Task<IActionResult> DownloadInternal(string token, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            new DownloadSharedProformPdfQuery(token),
            cancellationToken);

        Response.Headers.CacheControl = "no-store, no-cache";
        Response.Headers["Referrer-Policy"] = "no-referrer";

        return File(response.Content, response.ContentType, response.FileName);
    }
}
