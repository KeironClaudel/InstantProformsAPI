using MediatR;

namespace InstantProforms.Application.Features.Clients.GetClients;

/// <summary>
/// Represents a request to retrieve active clients for the current company.
/// </summary>
public sealed record GetClientsQuery() : IRequest<IReadOnlyCollection<ClientResponse>>;
