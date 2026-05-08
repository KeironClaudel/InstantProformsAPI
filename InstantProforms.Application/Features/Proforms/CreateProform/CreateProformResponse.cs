namespace InstantProforms.Application.Features.Proforms.CreateProform;

/// <summary>
/// Represents the result of creating a new proform.
/// </summary>
public sealed record CreateProformResponse(
    Guid ProformId,
    string Number,
    string Currency,
    string Status,
    decimal Subtotal,
    decimal TaxPercentage,
    decimal TaxAmount,
    decimal Total);
