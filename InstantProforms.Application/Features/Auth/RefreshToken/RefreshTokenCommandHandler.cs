using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Models;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Features.Auth.RefToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _context.RefreshTokens
            .Include(x => x.User)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (storedToken is null || !storedToken.IsActive || !storedToken.User.IsActive)
        {
            throw new InvalidOperationException("Invalid or expired refresh token.");
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;

        var newAccessToken = _jwtTokenService.GenerateAccessToken(storedToken.User);
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = storedToken.UserId,
            Token = newRefreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(newAccessToken, newRefreshTokenValue);
    }
}