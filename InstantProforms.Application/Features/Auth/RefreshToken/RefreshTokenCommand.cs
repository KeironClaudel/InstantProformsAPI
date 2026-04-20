using MediatR;

namespace InstantProforms.Application.Features.Auth.RefToken;

public sealed record RefreshTokenCommand(
    string RefreshToken) : IRequest<RefreshTokenResponse>;