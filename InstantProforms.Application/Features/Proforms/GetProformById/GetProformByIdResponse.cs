namespace InstantProforms.Application.Features.Proforms.GetProformById;

/// <summary>
/// Represents the details of a proform.
/// </summary>
public sealed record GetProformByIdResponse(
    Guid Id,
    string Number,
    string Status,
    string ClientName,
    string? ClientEmail,
    string? ClientPhone,
    DateTime IssuedAtUtc,
    string? Notes,
    decimal Subtotal,
    decimal TaxPercentage,
    decimal TaxAmount,
    decimal Total,
    IReadOnlyCollection<GetProformByIdItemResponse> Items);

/// <summary>
/// Represents a proform item detail.
/// </summary>
public sealed record GetProformByIdItemResponse(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Total,
    int SortOrder);