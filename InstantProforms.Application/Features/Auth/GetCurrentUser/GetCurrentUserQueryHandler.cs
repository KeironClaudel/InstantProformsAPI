using MediatR;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Interfaces;

namespace InstantProforms.Application.Features.Auth.GetCurrentUser;

/// <summary>
/// Handles retrieval of the currently authenticated user.
/// </summary>
public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPlatformAdminAccessService _platformAdminAccessService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCurrentUserQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public GetCurrentUserQueryHandler(
        IUnitOfWork unitOfWork,
        IPlatformAdminAccessService platformAdminAccessService)
    {
        _unitOfWork = unitOfWork;
        _platformAdminAccessService = platformAdminAccessService;
    }

    /// <inheritdoc />
    public async Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users
            .GetActiveByIdWithRoleAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Authenticated user was not found.");
        }

        return new GetCurrentUserResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.Name,
            user.CompanyId,
            _platformAdminAccessService.IsPlatformAdmin(user.Email));
    }
}
