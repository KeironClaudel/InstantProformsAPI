namespace InstantProforms.Api.Contracts.Proforms;

/// <summary>
/// Represents the HTTP request to update a proform status.
/// </summary>
public sealed record UpdateProformStatusRequest(
    string Status);