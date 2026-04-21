using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InstantProforms.Api.Contracts.CompanySettings;
using InstantProforms.Application.Features.CompanyConfig.GetCompanySettings;
using InstantProforms.Application.Features.CompanyConfig.UpsertCompanySettings;

namespace InstantProforms.Api.Controllers;

/// <summary>
/// Provides endpoints for company settings management.
/// </summary>
[ApiController]
[Authorize]
[Route("api/company-settings")]
public sealed class CompanySettingsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanySettingsController"/> class.
    /// </summary>
    public CompanySettingsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets the current company settings.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The company settings.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetCompanySettingsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetCompanySettingsResponse>> Get(CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetCompanySettingsQuery(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Creates or updates the current company settings.
    /// </summary>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The upsert result.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(UpsertCompanySettingsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpsertCompanySettingsResponse>> Upsert(
        [FromBody] UpsertCompanySettingsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);
        return Ok(response);
    }
}