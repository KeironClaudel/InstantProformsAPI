using MediatR;
using Microsoft.Extensions.Options;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Features.Auth.ForgotPassword;

/// <summary>
/// Handles forgot password requests.
/// </summary>
public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ITokenHashService _tokenHashService;
    private readonly PasswordResetSettings _passwordResetSettings;
    private readonly IJwtTokenService _jwtTokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordCommandHandler"/> class.
    /// </summary>
    public ForgotPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ITokenHashService tokenHashService,
        IOptions<PasswordResetSettings> passwordResetSettings,
        IJwtTokenService jwtTokenService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _tokenHashService = tokenHashService;
        _passwordResetSettings = passwordResetSettings.Value;
        _jwtTokenService = jwtTokenService;
    }

    /// <inheritdoc />
    public async Task<ForgotPasswordResponse> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users
            .GetByEmailWithRoleAsync(request.Email, cancellationToken);

        if (user is not null && user.IsActive)
        {
            var rawToken = _jwtTokenService.GenerateRefreshToken();
            var tokenHash = _tokenHashService.ComputeHash(rawToken);

            var passwordResetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_passwordResetSettings.TokenExpirationMinutes),
                CreatedAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.PasswordResetTokens.AddAsync(passwordResetToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var resetLink = $"{_passwordResetSettings.ResetUrl}?token={Uri.EscapeDataString(rawToken)}";

            var subject = "Reset your password";
            var body =
                $"""
                <p>Hello,</p>
                <p>We received a request to reset your password.</p>
                <p><a href="{resetLink}">Click here to reset your password</a></p>
                <p>This link expires in {_passwordResetSettings.TokenExpirationMinutes} minutes.</p>
                <p>If you did not request this, you can ignore this email.</p>
                """;

            await _emailService.SendAsync(
                user.CompanyId,
                user.Email,
                subject,
                body,
                attachmentFileName: null,
                attachmentContent: null,
                cancellationToken);
        }

        return new ForgotPasswordResponse(
            "If the email exists in the system, a password reset link has been sent.");
    }
}
