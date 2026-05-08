namespace InstantProforms.Api.Contracts.Clients;

/// <summary>
/// Represents the HTTP request to create a client.
/// </summary>
public sealed record CreateClientRequest(
    string Name,
    string? Email,
    string? Phone,
    string? IdentificationType,
    string? IdentificationNumber);
