using MediatR;

namespace InstantProforms.Application.Features.Proforms.CreateProform;

/// <summary>
/// Represents a request to create a new proform.
/// </summary>
public sealed record CreateProformCommand(
    string ClientName,
    string? ClientEmail,
    string? ClientPhone,
    string? Notes,
    IReadOnlyCollection<CreateProformItemModel> Items) : IRequest<CreateProformResponse>;

/// <summary>
/// Represents a line item to be created inside a proform.
/// </summary>
public sealed record CreateProformItemModel(
    string Description,
    decimal Quantity,
    decimal UnitPrice);