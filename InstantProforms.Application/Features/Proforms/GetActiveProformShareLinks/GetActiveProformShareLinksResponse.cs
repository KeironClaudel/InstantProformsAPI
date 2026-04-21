namespace InstantProforms.Application.Features.Proforms.GetActiveProformShareLinks;

/// <summary>
/// Represents the result of listing active share links for a proform.
/// </summary>
public sealed record GetActiveProformShareLinksResponse(
    Guid ProformId,
    IReadOnlyCollection<GetActiveProformShareLinkItemResponse> Items);

/// <summary>
/// Represents an active proform share link item.
/// </summary>
public sealed record GetActiveProformShareLinkItemResponse(
    Guid ShareTokenId,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc,
    bool IsSingleUse,
    bool HasBeenUsed);