namespace InstantProforms.Application.Features.Auth.GetCurrentUser;

public sealed record GetCurrentUserResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    Guid CompanyId);