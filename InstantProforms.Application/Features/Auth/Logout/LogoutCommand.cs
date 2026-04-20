using MediatR;

namespace InstantProforms.Application.Features.Auth.Logout;

public sealed record LogoutCommand(
    string RefreshToken) : IRequest;