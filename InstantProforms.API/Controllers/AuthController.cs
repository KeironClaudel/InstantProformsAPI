using InstantProforms.Api.Contracts.Auth;
using InstantProforms.Api.Extensions;
using InstantProforms.Application.Features.Auth.GetCurrentUser;
using InstantProforms.Application.Features.Auth.Logout;
using InstantProforms.Application.Features.Auth.RefToken;
using InstantProforms.Application.Features.Auth.RegisterCompany;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InstantProforms.Application.Features.Auth.ForgotPassword;
using InstantProforms.Application.Features.Auth.ResetPassword;

namespace InstantProforms.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register-company")]
    [ProducesResponseType(typeof(RegisterCompanyResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RegisterCompanyResponse>> RegisterCompany(
        [FromBody] RegisterCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);

        Response.AppendAccessTokenCookie(response.AccessToken);
        Response.AppendRefreshTokenCookie(response.RefreshToken);

        return Ok(new
        {
            response.UserId,
            response.FullName,
            response.Email,
            response.Role,
            response.CompanyId
        });
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token cookie was not found." });
        }

        var response = await _sender.Send(
            new RefreshTokenCommand(refreshToken),
            cancellationToken);

        Response.AppendAccessTokenCookie(response.AccessToken);
        Response.AppendRefreshTokenCookie(response.RefreshToken);

        return Ok(new { message = "Token refreshed successfully." });
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken) &&
            !string.IsNullOrWhiteSpace(refreshToken))
        {
            await _sender.Send(new LogoutCommand(refreshToken), cancellationToken);
        }

        Response.ClearAuthCookies();

        return Ok(new { message = "Logged out successfully." });
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(GetCurrentUserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetCurrentUserResponse>> Me(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(ClaimTypes.Name)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        var subClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(subClaim, out var userId))
        {
            return Unauthorized(new { message = "User identifier claim is missing or invalid." });
        }

        var response = await _sender.Send(new GetCurrentUserQuery(userId), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Starts the forgot password flow.
    /// </summary>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A generic success response.</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Resets a user password using a valid reset token.
    /// </summary>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The password reset result.</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResetPasswordResponse>> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);
        Response.ClearAuthCookies();
        return Ok(response);
    }
}