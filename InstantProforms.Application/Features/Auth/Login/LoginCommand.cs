using MediatR;

namespace InstantProforms.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    bool RememberMe) : IRequest<LoginResponse>;
