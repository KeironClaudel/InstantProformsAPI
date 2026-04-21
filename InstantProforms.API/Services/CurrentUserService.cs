using System.Security.Claims;
using InstantProforms.Application.Common.Interfaces;

namespace InstantProforms.Api.Services;

/// <summary>
/// Provides access to current authenticated user claims.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var result) ? result : null;
        }
    }

    /// <inheritdoc />
    public Guid? CompanyId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue("companyId");
            return Guid.TryParse(value, out var result) ? result : null;
        }
    }

    /// <inheritdoc />
    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}