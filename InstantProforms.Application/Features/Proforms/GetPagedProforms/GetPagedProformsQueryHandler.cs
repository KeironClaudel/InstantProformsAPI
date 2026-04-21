using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using MediatR;

namespace InstantProforms.Application.Features.Proforms.GetPagedProforms;

/// <summary>
/// Handles paginated retrieval of Proforms.
/// </summary>
public sealed class GetPagedProformsQueryHandler
    : IRequestHandler<GetPagedProformsQuery, GetPagedProformsResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPagedProformsQueryHandler"/> class.
    /// </summary>
    public GetPagedProformsQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    public async Task<GetPagedProformsResponse> Handle(
        GetPagedProformsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var (items, totalCount) = await _unitOfWork.Proforms
            .GetPagedAsync(
                _currentUserService.CompanyId.Value,
                request.Page,
                request.PageSize,
                cancellationToken);

        return new GetPagedProformsResponse(
            items.Select(x => new GetPagedProformsItemResponse(
                x.Id,
                x.Number,
                x.ClientName,
                x.IssuedAtUtc,
                x.Total))
            .ToList(),
            request.Page,
            request.PageSize,
            totalCount);
    }
}