using MediatR;

namespace InstantProforms.Application.Features.Proforms.GetActiveProformShareLinks;

/// <summary>
/// Represents a request to get active share links for a proform.
/// </summary>
public sealed record GetActiveProformShareLinksQuery(
    Guid ProformId) : IRequest<GetActiveProformShareLinksResponse>;