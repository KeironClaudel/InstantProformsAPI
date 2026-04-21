namespace InstantProforms.Api.Contracts.Proforms;

/// <summary>
/// Represents the HTTP request to create a proform.
/// </summary>
public sealed record CreateProformRequest(
    string ClientName,
    string? ClientEmail,
    string? ClientPhone,
    string? Notes,
    IReadOnlyCollection<CreateProformItemRequest> Items);

/// <summary>
/// Represents a line item in the create proform request.
/// </summary>
public sealed record CreateProformItemRequest(
    string Description,
    decimal Quantity,
    decimal UnitPrice);