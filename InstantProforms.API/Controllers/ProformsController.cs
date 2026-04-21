using InstantProforms.Api.Contracts.Proforms;
using InstantProforms.Application.Features.Proforms.CreateProform;
using InstantProforms.Application.Features.Proforms.GetPagedProforms;
using InstantProforms.Application.Features.Proforms.GetProformById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InstantProforms.Application.Features.Proforms.UpdateProformStatus;
using InstantProforms.Application.Features.Proforms.DownloadProformPdf;
using InstantProforms.Application.Features.Proforms.SendProformByEmail;

namespace InstantProforms.Api.Controllers;

/// <summary>
/// Provides endpoints for proform management.
/// </summary>
[ApiController]
[Authorize]
[Route("api/Proforms")]
public sealed class ProformsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProformsController"/> class.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public ProformsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new proform.
    /// </summary>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created proform result.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateProformResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateProformResponse>> Create(
        [FromBody] CreateProformRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Gets a proform by identifier.
    /// </summary>
    /// <param name="id">The proform identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The proform detail.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetProformByIdResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetProformByIdResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetProformByIdQuery(id), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets paginated Proform for the current company.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of Proforms.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetPagedProformsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPagedProformsResponse>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _sender.Send(
            new GetPagedProformsQuery(page, pageSize),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Updates the status of a proform.
    /// </summary>
    /// <param name="id">The proform identifier.</param>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated status result.</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(UpdateProformStatusResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateProformStatusResponse>> UpdateStatus(
        Guid id,
        [FromBody] UpdateProformStatusRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(id), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Downloads a PDF representation of a proform.
    /// </summary>
    /// <param name="id">The proform identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated PDF file.</returns>
    [HttpGet("{id:guid}/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadPdf(Guid id, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new DownloadProformPdfQuery(id), cancellationToken);

        return File(response.Content, response.ContentType, response.FileName);
    }

    /// <summary>
    /// Sends a proform by email with a PDF attachment.
    /// </summary>
    /// <param name="id">The proform identifier.</param>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email delivery result.</returns>
    [HttpPost("{id:guid}/send-email")]
    [ProducesResponseType(typeof(SendProformByEmailResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SendProformByEmailResponse>> SendByEmail(
        Guid id,
        [FromBody] SendProformByEmailRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(id), cancellationToken);
        return Ok(response);
    }
}