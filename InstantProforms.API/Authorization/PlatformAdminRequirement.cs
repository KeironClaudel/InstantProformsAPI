using Microsoft.AspNetCore.Authorization;

namespace InstantProforms.Api.Authorization;

/// <summary>
/// Requires the current authenticated user to be recognized as a platform administrator.
/// </summary>
public sealed class PlatformAdminRequirement : IAuthorizationRequirement
{
}
