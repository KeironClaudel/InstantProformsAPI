namespace InstantProforms.Api.Contracts.Proforms;

/// <summary>
/// Represents the HTTP request to create a temporary public share link for a proform.
/// </summary>
public sealed record CreateProformShareLinkRequest(
    bool IsSingleUse = false,
    int? ExpirationMinutes = null);