namespace InstantProforms.Application.Features.Clients;

/// <summary>
/// Represents a client payload returned by the API.
/// </summary>
public sealed record ClientResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    string? IdentificationType,
    string? IdentificationNumber);
