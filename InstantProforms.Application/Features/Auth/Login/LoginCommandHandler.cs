using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Models;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Features.Auth.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == request.Email && x.IsActive, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByIp = null
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            accessToken,
            refreshTokenValue,
            user.Id,
            user.FullName,
            user.Email,
            user.Role.Name,
            user.CompanyId);
    }
}