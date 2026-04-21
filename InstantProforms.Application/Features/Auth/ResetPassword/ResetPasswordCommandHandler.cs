using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;

namespace InstantProforms.Application.Features.Auth.ResetPassword;

/// <summary>
/// Handles password reset operations.
/// </summary>
public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenHashService _tokenHashService;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordCommandHandler"/> class.
    /// </summary>
    public ResetPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenHashService tokenHashService,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _tokenHashService = tokenHashService;
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc />
    public async Task<ResetPasswordResponse> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = _tokenHashService.ComputeHash(request.Token);

        var passwordResetToken = await _unitOfWork.PasswordResetTokens
            .GetByTokenHashWithUserAsync(tokenHash, cancellationToken);

        if (passwordResetToken is null || !passwordResetToken.IsActive || !passwordResetToken.User.IsActive)
        {
            throw new InvalidOperationException("Invalid or expired password reset token.");
        }

        passwordResetToken.User.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        passwordResetToken.User.UpdatedAtUtc = DateTime.UtcNow;
        passwordResetToken.UsedAtUtc = DateTime.UtcNow;

        var activeRefreshTokens = await _unitOfWork.RefreshTokens
            .GetActiveByUserIdAsync(passwordResetToken.UserId, cancellationToken);

        foreach (var refreshToken in activeRefreshTokens)
        {
            refreshToken.RevokedAtUtc = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ResetPasswordResponse("Password reset successfully.");
    }
}