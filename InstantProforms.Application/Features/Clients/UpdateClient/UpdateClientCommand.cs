using InstantProforms.Domain.Enums;
using MediatR;

namespace InstantProforms.Application.Features.Clients.UpdateClient;

/// <summary>
/// Represents a request to update a client.
/// </summary>
public sealed record UpdateClientCommand(
    Guid ClientId,
    string Name,
    string? Email,
    string? Phone,
    ClientIdentificationType? IdentificationType,
    string? IdentificationNumber) : IRequest<ClientResponse>;
