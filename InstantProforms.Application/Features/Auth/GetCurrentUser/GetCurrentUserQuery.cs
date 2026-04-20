using MediatR;

namespace InstantProforms.Application.Features.Auth.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<GetCurrentUserResponse>;