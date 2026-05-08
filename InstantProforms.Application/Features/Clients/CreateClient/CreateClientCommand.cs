using InstantProforms.Domain.Enums;
using MediatR;

namespace InstantProforms.Application.Features.Clients.CreateClient;

/// <summary>
/// Represents a request to create a client.
/// </summary>
public sealed record CreateClientCommand(
    string Name,
    string? Email,
    string? Phone,
    ClientIdentificationType? IdentificationType,
    string? IdentificationNumber) : IRequest<ClientResponse>;
