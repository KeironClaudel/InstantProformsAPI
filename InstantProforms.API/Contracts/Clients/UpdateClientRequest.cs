namespace InstantProforms.Api.Contracts.Clients;

/// <summary>
/// Represents the HTTP request to update a client.
/// </summary>
public sealed record UpdateClientRequest(
    string Name,
    string? Email,
    string? Phone,
    string? IdentificationType,
    string? IdentificationNumber);
