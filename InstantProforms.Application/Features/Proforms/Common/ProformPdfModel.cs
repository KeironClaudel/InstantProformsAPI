namespace InstantProforms.Application.Features.Proforms.Common;

/// <summary>
/// Represents the data required to render a proform PDF.
/// </summary>
public sealed record ProformPdfModel(
    string DisplayName,
    string? Website,
    string? Phone,
    string? Email,
    string? Address,
    string? LogoFileName,
    string? PrimaryColor,
    string? SecondaryColor,
    string? AccentColor,
    string CurrencySymbol,
    string TaxLabel,
    string? TermsAndConditions,
    Guid ProformId,
    string Number,
    string Status,
    string ClientName,
    string? ClientEmail,
    string? ClientPhone,
    decimal TaxPercentage,
    decimal TaxAmount,
    DateTime IssuedAtUtc,
    string? Notes,
    decimal Subtotal,
    decimal Total,
    IReadOnlyCollection<ProformPdfItemModel> Items);

/// <summary>
/// Represents a line item in a proform PDF.
/// </summary>
public sealed record ProformPdfItemModel(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Total,
    int SortOrder);