
using MediatR;

namespace InstantProforms.Application.Features.Proforms.GetProformById;

/// <summary>
/// Represents a request to get a proform by identifier.
/// </summary>
public sealed record GetProformByIdQuery(Guid ProformId) : IRequest<GetProformByIdResponse>;