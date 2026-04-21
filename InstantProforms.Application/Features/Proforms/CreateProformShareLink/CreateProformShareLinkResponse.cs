namespace InstantProforms.Application.Features.Proforms.CreateProformShareLink;

/// <summary>
/// Represents the result of creating a proform share link.
/// </summary>
public sealed record CreateProformShareLinkResponse(
    string Url,
    DateTime ExpiresAtUtc,
    bool IsSingleUse);