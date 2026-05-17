namespace InstantProforms.Application.Features.Auth.Login;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    bool IsPersistent,
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    Guid CompanyId);
