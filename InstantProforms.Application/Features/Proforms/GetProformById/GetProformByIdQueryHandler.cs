using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using MediatR;

namespace InstantProforms.Application.Features.Proforms.GetProformById;

/// <summary>
/// Handles retrieval of a proforms by identifier.
/// </summary>
public sealed class GetProformByIdQueryHandler
    : IRequestHandler<GetProformByIdQuery, GetProformByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProformByIdQueryHandler"/> class.
    /// </summary>
    public GetProformByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    public async Task<GetProformByIdResponse> Handle(
        GetProformByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var proform = await _unitOfWork.Proforms
            .GetByIdWithItemsAsync(request.ProformId, _currentUserService.CompanyId.Value, cancellationToken);

        if (proform is null)
        {
            throw new InvalidOperationException("Proform was not found.");
        }

        return new GetProformByIdResponse(
                proform.Id,
                proform.Number,
                proform.Status.ToString(),
                proform.ClientName,
                proform.ClientEmail,
                proform.ClientPhone,
                proform.IssuedAtUtc,
                proform.Notes,
                proform.Subtotal,
                proform.TaxPercentage,
                proform.TaxAmount,
                proform.Total,
                proform.Items
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new GetProformByIdItemResponse(
                        x.Id,
                        x.Description,
                        x.Quantity,
                        x.UnitPrice,
                        x.Total,
                        x.SortOrder))
                    .ToList());
    }
}
