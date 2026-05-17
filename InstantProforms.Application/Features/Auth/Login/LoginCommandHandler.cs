using MediatR;
using Microsoft.Extensions.Options;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Features.Auth.Login;

/// <summary>
/// Handles user login and token issuance.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITokenHashService _tokenHashService;
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="jwtTokenService">The JWT token service.</param>
    /// <param name="tokenHashService">The token hash service.</param>
    /// <param name="jwtSettings">The JWT configuration settings.</param>
    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ITokenHashService tokenHashService,
        IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _tokenHashService = tokenHashService;
        _jwtSettings = jwtSettings.Value;
    }

    /// <inheritdoc />
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users
            .GetActiveByEmailWithRoleAsync(request.Email, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = _tokenHashService.ComputeHash(refreshTokenValue);
        var rememberMeRefreshTokenLifetimeDays = _jwtSettings.RememberMeRefreshTokenExpirationDays is > 0
            ? _jwtSettings.RememberMeRefreshTokenExpirationDays.Value
            : _jwtSettings.RefreshTokenExpirationDays;
        var refreshTokenLifetimeDays = request.RememberMe
            ? rememberMeRefreshTokenLifetimeDays
            : _jwtSettings.RefreshTokenExpirationDays;

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenHash,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays),
            IsPersistent = request.RememberMe,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            accessToken,
            refreshTokenValue,
            request.RememberMe,
            user.Id,
            user.FullName,
            user.Email,
            user.Role.Name,
            user.CompanyId);
    }
}
