namespace InstantProforms.Application.Features.Proforms.GetPagedProforms;

/// <summary>
/// Represents a paginated proform result.
/// </summary>
public sealed record GetPagedProformsResponse(
    IReadOnlyCollection<GetPagedProformsItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);

/// <summary>
/// Represents a proform summary item.
/// </summary>
public sealed record GetPagedProformsItemResponse(
    Guid Id,
    string Number,
    string Status,
    string ClientName,
    DateTime IssuedAtUtc,
    string? ClientEmail,
    string? ClientPhone,
    decimal Subtotal,
    decimal TaxPercentage,
    decimal TaxAmount,
    decimal Total);