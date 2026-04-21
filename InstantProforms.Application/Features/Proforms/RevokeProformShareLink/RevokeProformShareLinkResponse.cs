namespace InstantProforms.Application.Features.Proforms.RevokeProformShareLink;

/// <summary>
/// Represents the result of revoking a proform share link.
/// </summary>
public sealed record RevokeProformShareLinkResponse(
    Guid ShareTokenId,
    string Message);