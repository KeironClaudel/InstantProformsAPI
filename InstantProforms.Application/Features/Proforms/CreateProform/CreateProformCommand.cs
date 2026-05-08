using InstantProforms.Domain.Enums;
using MediatR;

namespace InstantProforms.Application.Features.Proforms.CreateProform;

/// <summary>
/// Represents a request to create a new proform.
/// </summary>
public sealed record CreateProformCommand(
    Guid? ClientId,
    string ClientName,
    string? ClientEmail,
    string? ClientPhone,
    string? Notes,
    string? Location,
    string? InternalNotes,
    ClientIdentificationType? ClientIdentificationType,
    string? ClientIdentificationNumber,
    ProformCurrency? Currency,
    string? ServiceDescription,
    string? ScopeOfWork,
    string? ServiceConditions,
    string? PaymentConditions,
    IReadOnlyCollection<CreateProformItemModel> Items) : IRequest<CreateProformResponse>;

/// <summary>
/// Represents a line item to be created inside a proform.
/// </summary>
public sealed record CreateProformItemModel(
    string Description,
    decimal Quantity,
    decimal UnitPrice);
