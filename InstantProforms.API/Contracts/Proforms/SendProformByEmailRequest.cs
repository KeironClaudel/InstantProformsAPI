namespace InstantProforms.Api.Contracts.Proforms;

/// <summary>
/// Represents the HTTP request to send a proform by email.
/// </summary>
public sealed record SendProformByEmailRequest(
    string ToEmail,
    string? Subject,
    string? Message);