using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;

namespace InstantProforms.Application.Features.Auth.Logout;

/// <summary>
/// Handles refresh token revocation during logout.
/// </summary>
public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenHashService _tokenHashService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="tokenHashService">The token hash service.</param>
    public LogoutCommandHandler(IUnitOfWork unitOfWork, ITokenHashService tokenHashService)
    {
        _unitOfWork = unitOfWork;
        _tokenHashService = tokenHashService;
    }

    /// <inheritdoc />
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var providedTokenHash = _tokenHashService.ComputeHash(request.RefreshToken);

        var storedToken = await _unitOfWork.RefreshTokens
            .GetByTokenHashAsync(providedTokenHash, cancellationToken);

        if (storedToken is not null && storedToken.RevokedAtUtc is null)
        {
            storedToken.RevokedAtUtc = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
