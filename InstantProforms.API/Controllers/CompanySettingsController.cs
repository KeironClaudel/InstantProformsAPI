using InstantProforms.Api.Common.Helpers;
using InstantProforms.Api.Contracts.CompanySettings;
using InstantProforms.Application.Features.CompanyConfig.GetCompanyLogo;
using InstantProforms.Application.Features.CompanyConfig.GetCompanySettings;
using InstantProforms.Application.Features.CompanyConfig.ReplaceLogo;
using InstantProforms.Application.Features.CompanyConfig.UpsertCompanySettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        var logoUrl = BuildLogoEndpointUrl(response.LogoFileName);

        return Ok(response with { LogoUrl = logoUrl });
    }

    /// <summary>
    /// Gets the current company logo image.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current company logo file.</returns>
    [HttpGet("logo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLogo(CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetCompanyLogoQuery(), cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        Response.Headers.CacheControl = "private, max-age=300";

        return File(response.Content, response.ContentType);
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

    [HttpPut("logo")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReplaceLogo(
    [FromForm] ReplaceCompanyLogoRequest request,
    CancellationToken cancellationToken)
    {
        await _sender.Send(new ReplaceCompanyLogoCommand(request.LogoFile), cancellationToken);

        return NoContent();
    }

    private string? BuildLogoEndpointUrl(string? logoFileName)
    {
        if (string.IsNullOrWhiteSpace(logoFileName))
        {
            return null;
        }

        var version = Uri.EscapeDataString(logoFileName);
        var pathBase = Request.PathBase.HasValue ? Request.PathBase.Value : string.Empty;

        return $"{Request.Scheme}://{Request.Host}{pathBase}/api/company-settings/logo?v={version}";
    }
}
