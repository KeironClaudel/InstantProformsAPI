using MediatR;
using InstantProforms.Application.Common.Interfaces.Persistence;

namespace InstantProforms.Application.Features.Auth.Logout;

/// <summary>
/// Handles refresh token revocation during logout.
/// </summary>
public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public LogoutCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _unitOfWork.RefreshTokens
            .GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (storedToken is not null && storedToken.RevokedAtUtc is null)
        {
            storedToken.RevokedAtUtc = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}