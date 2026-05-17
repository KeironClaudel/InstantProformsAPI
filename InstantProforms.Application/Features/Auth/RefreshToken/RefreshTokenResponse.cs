namespace InstantProforms.Application.Features.Auth.RefToken;

public sealed record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    bool IsPersistent);
