using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using MediatR;

namespace InstantProforms.Application.Features.Clients.GetClientById;

/// <summary>
/// Handles retrieval of a client by identifier.
/// </summary>
public sealed class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetClientByIdQueryHandler"/> class.
    /// </summary>
    public GetClientByIdQueryHandler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ClientResponse> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
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

        return new ClientResponse(
            client.Id,
            client.Name,
            client.Email,
            client.Phone,
            client.IdentificationType?.ToString(),
            client.IdentificationNumber);
    }
}
