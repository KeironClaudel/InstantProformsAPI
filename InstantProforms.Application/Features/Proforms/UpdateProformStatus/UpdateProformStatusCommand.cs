using MediatR;

namespace InstantProforms.Application.Features.Proforms.UpdateProformStatus;

/// <summary>
/// Represents a request to update a proform status.
/// </summary>
public sealed record UpdateProformStatusCommand(
    Guid ProformId,
    string Status) : IRequest<UpdateProformStatusResponse>;