using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using MediatR;

namespace InstantProforms.Application.Features.Clients.DeleteClient;

/// <summary>
/// Handles client archival.
/// </summary>
public sealed class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteClientCommandHandler"/> class.
    /// </summary>
    public DeleteClientCommandHandler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var client = await _unitOfWork.Clients.GetByIdAsync(
            request.ClientId,
            _currentUserService.CompanyId.Value,
            cancellationToken);

        if (client is null)
        {
            throw new InvalidOperationException("Client was not found.");
        }

        client.IsActive = false;
        client.UpdatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
