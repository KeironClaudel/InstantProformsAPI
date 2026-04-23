using InstantProforms.Api.Contracts.Auth;
using InstantProforms.Api.Services;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Features.Auth.ForgotPassword;
using InstantProforms.Application.Features.Auth.GetCurrentUser;
using InstantProforms.Application.Features.Auth.Logout;
using InstantProforms.Application.Features.Auth.RefToken;
using InstantProforms.Application.Features.Auth.RegisterCompany;
using InstantProforms.Application.Features.Auth.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace InstantProforms.Api.Controllers;

/// <summary>
/// Controller responsible for handling authentication-related operations such as user registration, login, token refresh, logout, and password management.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IAuthCookieService _authCookieService;
    private readonly ICsrfTokenService _csrfTokenService;

    public AuthController(
        ISender sender,
        ICsrfTokenService csrfTokenService,
        IAuthCookieService authCookieService)
    {
        _sender = sender;
        _authCookieService = authCookieService;
        _csrfTokenService = csrfTokenService;
    }

    /// <summary>
    /// Registers a new company along with an initial admin user. This endpoint is typically used for onboarding new customers to the platform.
    /// </summary>
    /// <param name="request">The registration request payload containing company and admin user details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the registration process, including the new company and user identifiers.</returns>
    [EnableRateLimiting("auth-strict")]
    [HttpPost("register-company")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterCompanyResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RegisterCompanyResponse>> RegisterCompany(
        [FromForm] RegisterCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Authenticates a user and issues access and refresh tokens, which are set in secure HTTP-only cookies.
    /// </summary>
    /// <param name="request">The login request payload containing user credentials.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authenticated user's information along with the issued tokens set in cookies.</returns>
    [EnableRateLimiting("auth-strict")]
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);

        var csrfToken = _csrfTokenService.GenerateToken();
        _authCookieService.AppendSessionCookies(Response, response.AccessToken, response.RefreshToken, csrfToken);

        return Ok(new
        {
            response.UserId,
            response.FullName,
            response.Email,
            response.Role,
            response.CompanyId
        });
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token from the cookie.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A generic success response with new tokens set in cookies.</returns>
    [EnableRateLimiting("auth-strict")]
    [HttpPost("refresh")]
    [AllowAnonymous]
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

        var csrfToken = _csrfTokenService.GenerateToken();
        _authCookieService.AppendSessionCookies(Response, response.AccessToken, response.RefreshToken, csrfToken);

        return Ok(new { message = "Token refreshed successfully." });
    }

    /// <summary>
    /// Logs out the current user by invalidating the refresh token and clearing authentication cookies.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A generic success response indicating the user has been logged out.</returns>
    [EnableRateLimiting("auth-medium")]
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken) &&
            !string.IsNullOrWhiteSpace(refreshToken))
        {
            await _sender.Send(new LogoutCommand(refreshToken), cancellationToken);
        }

        _authCookieService.ClearSessionCookies(Response);

        return Ok(new { message = "Logged out successfully." });
    }

    /// <summary>
    /// Gets the current authenticated user's information.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current user's details.</returns>
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
    /// <returns>The result of the forgot password request, typically indicating that an email has been sent if the user exists.</returns>
    [EnableRateLimiting("auth-strict")]
    [HttpPost("forgot-password")]
    [AllowAnonymous]
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
    /// <returns>The result of the reset password request, typically indicating that the password has been successfully reset.</returns>
    [EnableRateLimiting("auth-strict")]
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResetPasswordResponse>> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);
        _authCookieService.ClearSessionCookies(Response);
        return Ok(response);
    }
}
