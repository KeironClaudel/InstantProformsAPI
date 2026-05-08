using MediatR;

namespace InstantProforms.Application.Features.Clients.DeleteClient;

/// <summary>
/// Represents a request to archive a client.
/// </summary>
public sealed record DeleteClientCommand(Guid ClientId) : IRequest;
