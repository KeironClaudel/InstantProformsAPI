using MediatR;

namespace InstantProforms.Application.Features.Clients.GetClientById;

/// <summary>
/// Represents a request to retrieve a client by identifier.
/// </summary>
public sealed record GetClientByIdQuery(Guid ClientId) : IRequest<ClientResponse>;
