using MediatR;
using Microsoft.Extensions.Options;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Features.Auth.RefToken;

/// <summary>
/// Handles refresh token rotation and access token renewal.
/// </summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITokenHashService _tokenHashService;
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="jwtTokenService">The JWT token service.</param>
    /// <param name="tokenHashService">The token hash service.</param>
    /// <param name="jwtSettings">The JWT configuration settings.</param>
    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        ITokenHashService tokenHashService,
        IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _tokenHashService = tokenHashService;
        _jwtSettings = jwtSettings.Value;
    }

    /// <inheritdoc />
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var providedTokenHash = _tokenHashService.ComputeHash(request.RefreshToken);

        var storedToken = await _unitOfWork.RefreshTokens
            .GetByTokenHashWithUserAsync(providedTokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive || !storedToken.User.IsActive)
        {
            throw new InvalidOperationException("Invalid or expired refresh token.");
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;

        var newAccessToken = _jwtTokenService.GenerateAccessToken(storedToken.User);
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _tokenHashService.ComputeHash(newRefreshTokenValue);
        var rememberMeRefreshTokenLifetimeDays = _jwtSettings.RememberMeRefreshTokenExpirationDays is > 0
            ? _jwtSettings.RememberMeRefreshTokenExpirationDays.Value
            : _jwtSettings.RefreshTokenExpirationDays;
        var refreshTokenLifetimeDays = storedToken.IsPersistent
            ? rememberMeRefreshTokenLifetimeDays
            : _jwtSettings.RefreshTokenExpirationDays;

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = storedToken.UserId,
            Token = newRefreshTokenHash,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays),
            IsPersistent = storedToken.IsPersistent,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(newAccessToken, newRefreshTokenValue, storedToken.IsPersistent);
    }
}
