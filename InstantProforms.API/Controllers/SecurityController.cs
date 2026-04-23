using InstantProforms.Api.Services;
using InstantProforms.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstantProforms.Api.Controllers;

/// <summary>
/// Provides security-related endpoints.
/// </summary>
[ApiController]
[Authorize]
[Route("api/security")]
public sealed class SecurityController : ControllerBase
{
    private readonly IAuthCookieService _authCookieService;
    private readonly ICsrfTokenService _csrfTokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityController"/> class.
    /// </summary>
    /// <param name="csrfTokenService">The CSRF token service.</param>
    /// <param name="authCookieService">The auth cookie service.</param>
    public SecurityController(
        ICsrfTokenService csrfTokenService,
        IAuthCookieService authCookieService)
    {
        _csrfTokenService = csrfTokenService;
        _authCookieService = authCookieService;
    }

    /// <summary>
    /// Issues a CSRF token for the current authenticated session.
    /// </summary>
    /// <returns>The token payload.</returns>
    [HttpGet("csrf-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCsrfToken()
    {
        var token = _csrfTokenService.GenerateToken();

        _authCookieService.AppendCsrfCookie(Response, token);

        return Ok(new
        {
            token
        });
    }
}
