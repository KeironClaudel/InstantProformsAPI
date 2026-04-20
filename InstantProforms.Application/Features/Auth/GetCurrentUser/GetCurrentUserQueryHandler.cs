using MediatR;
using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces;

namespace InstantProforms.Application.Features.Auth.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    private readonly IApplicationDbContext _context;

    public GetCurrentUserQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == request.UserId && x.IsActive, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Authenticated user was not found.");
        }

        return new GetCurrentUserResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.Name,
            user.CompanyId);
    }
}