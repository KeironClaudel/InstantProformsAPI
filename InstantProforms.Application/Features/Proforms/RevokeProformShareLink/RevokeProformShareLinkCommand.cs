using MediatR;

namespace InstantProforms.Application.Features.Proforms.RevokeProformShareLink;

/// <summary>
/// Represents a request to revoke an active proform share link.
/// </summary>
public sealed record RevokeProformShareLinkCommand(
    Guid ProformId,
    Guid ShareTokenId) : IRequest<RevokeProformShareLinkResponse>;