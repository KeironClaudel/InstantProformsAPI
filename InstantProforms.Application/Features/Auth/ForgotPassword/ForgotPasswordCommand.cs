using MediatR;

namespace InstantProforms.Application.Features.Auth.ForgotPassword;

/// <summary>
/// Represents a request to start the forgot password flow.
/// </summary>
public sealed record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResponse>;