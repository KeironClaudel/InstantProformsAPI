using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;

namespace InstantProforms.Application.Features.Proforms.RevokeProformShareLink;

/// <summary>
/// Handles revocation of an active proform share link.
/// </summary>
public sealed class RevokeProformShareLinkCommandHandler
    : IRequestHandler<RevokeProformShareLinkCommand, RevokeProformShareLinkResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeProformShareLinkCommandHandler"/> class.
    /// </summary>
    public RevokeProformShareLinkCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    public async Task<RevokeProformShareLinkResponse> Handle(
        RevokeProformShareLinkCommand request,
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
            throw new InvalidOperationException("Quotation was not found.");
        }

        var shareToken = await _unitOfWork.ProformShareTokens
            .GetActiveByIdAsync(request.ShareTokenId, request.ProformId, cancellationToken);

        if (shareToken is null)
        {
            throw new InvalidOperationException("Active share link was not found.");
        }

        shareToken.ExpiresAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RevokeProformShareLinkResponse(
            shareToken.Id,
            "Quotation share link revoked successfully.");
    }
}