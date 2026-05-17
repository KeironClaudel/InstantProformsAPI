using InstantProforms.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InstantProforms.Api.Authorization;

/// <summary>
/// Handles <see cref="PlatformAdminRequirement"/> authorization decisions.
/// </summary>
public sealed class PlatformAdminRequirementHandler : AuthorizationHandler<PlatformAdminRequirement>
{
    private readonly IPlatformAdminAccessService _platformAdminAccessService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformAdminRequirementHandler"/> class.
    /// </summary>
    /// <param name="platformAdminAccessService">The platform admin access service.</param>
    public PlatformAdminRequirementHandler(IPlatformAdminAccessService platformAdminAccessService)
    {
        _platformAdminAccessService = platformAdminAccessService;
    }

    /// <inheritdoc />
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PlatformAdminRequirement requirement)
    {
        var email = context.User.FindFirstValue(ClaimTypes.Email);

        if (_platformAdminAccessService.IsPlatformAdmin(email))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
