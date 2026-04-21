using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;

namespace InstantProforms.Application.Features.Proforms.GetActiveProformShareLinks;

/// <summary>
/// Handles retrieval of active share links for a proform.
/// </summary>
public sealed class GetActiveProformShareLinksQueryHandler
    : IRequestHandler<GetActiveProformShareLinksQuery, GetActiveProformShareLinksResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetActiveProformShareLinksQueryHandler"/> class.
    /// </summary>
    public GetActiveProformShareLinksQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    public async Task<GetActiveProformShareLinksResponse> Handle(
        GetActiveProformShareLinksQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var proform = await _unitOfWork.Proforms
            .GetByIdAsync(request.ProformId, _currentUserService.CompanyId.Value, cancellationToken);

        if (proform is null)
        {
            throw new InvalidOperationException("Proform was not found.");
        }

        var activeShareTokens = await _unitOfWork.ProformShareTokens
            .GetActiveByProformIdAsync(request.ProformId, cancellationToken);

        return new GetActiveProformShareLinksResponse(
            request.ProformId,
            activeShareTokens
                .Select(x => new GetActiveProformShareLinkItemResponse(
                    x.Id,
                    x.CreatedAtUtc,
                    x.ExpiresAtUtc,
                    x.IsSingleUse,
                    x.UsedAtUtc != null))
                .ToList());
    }
}