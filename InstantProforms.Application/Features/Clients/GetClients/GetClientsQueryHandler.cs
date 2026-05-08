using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using MediatR;

namespace InstantProforms.Application.Features.Clients.GetClients;

/// <summary>
/// Handles retrieval of active clients for the current company.
/// </summary>
public sealed class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, IReadOnlyCollection<ClientResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetClientsQueryHandler"/> class.
    /// </summary>
    public GetClientsQueryHandler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ClientResponse>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var clients = await _unitOfWork.Clients.GetActiveByCompanyAsync(_currentUserService.CompanyId.Value, cancellationToken);

        return clients
            .Select(x => new ClientResponse(
                x.Id,
                x.Name,
                x.Email,
                x.Phone,
                x.IdentificationType?.ToString(),
                x.IdentificationNumber))
            .ToList();
    }
}
