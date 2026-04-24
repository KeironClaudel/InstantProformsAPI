using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Enums;

namespace InstantProforms.Application.Features.Proforms.UpdateProformStatus;

/// <summary>
/// Handles proform status updates.
/// </summary>
public sealed class UpdateProformStatusCommandHandler
    : IRequestHandler<UpdateProformStatusCommand, UpdateProformStatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProformStatusCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="currentUserService">The current user service.</param>
    public UpdateProformStatusCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    public async Task<UpdateProformStatusResponse> Handle(
        UpdateProformStatusCommand request,
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

        if (!Enum.TryParse<ProformStatus>(request.Status, true, out var newStatus))
        {
            throw new InvalidOperationException("Invalid proform status.");
        }

        ValidateStatusTransition(proform.Status, newStatus);

        proform.Status = newStatus;
        proform.UpdatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateProformStatusResponse(
            proform.Id,
            proform.Status.ToString(),
            "Proform status updated successfully.");
    }

    private static void ValidateStatusTransition(ProformStatus currentStatus, ProformStatus newStatus)
    {
        if (currentStatus == newStatus)
        {
            throw new InvalidOperationException("The proform already has the requested status.");
        }
    }
}
