namespace InstantProforms.Application.Features.Proforms.UpdateProformStatus;

/// <summary>
/// Represents the result of updating a proform status.
/// </summary>
public sealed record UpdateProformStatusResponse(
    Guid ProformId,
    string Status,
    string Message);