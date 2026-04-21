using MediatR;

namespace InstantProforms.Application.Features.Auth.ResetPassword;

/// <summary>
/// Represents a request to reset a user password.
/// </summary>
public sealed record ResetPasswordCommand(
    string Token,
    string NewPassword) : IRequest<ResetPasswordResponse>;