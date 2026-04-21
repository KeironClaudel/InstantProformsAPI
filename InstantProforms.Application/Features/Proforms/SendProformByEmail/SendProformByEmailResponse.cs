namespace InstantProforms.Application.Features.Proforms.SendProformByEmail;

/// <summary>
/// Represents the result of sending a proform by email.
/// </summary>
public sealed record SendProformByEmailResponse(
    Guid ProformId,
    string Status,
    string Message);