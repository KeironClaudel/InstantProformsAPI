using MediatR;

namespace InstantProforms.Application.Features.Proforms.CreateProformShareLink;

/// <summary>
/// Represents a request to create a temporary public share link for a proform.
/// </summary>
public sealed record CreateProformShareLinkCommand(
    Guid ProformId,
    bool IsSingleUse = false,
    int? ExpirationMinutes = null) : IRequest<CreateProformShareLinkResponse>;