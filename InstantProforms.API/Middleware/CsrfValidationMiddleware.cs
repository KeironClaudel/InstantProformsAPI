using InstantProforms.Api.Common.Extensions;

namespace InstantProforms.Api.Middleware;

/// <summary>
/// Validates CSRF tokens for authenticated state-changing requests.
/// </summary>
public sealed class CsrfValidationMiddleware
{
    private static readonly HashSet<string> ProtectedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete
    };

    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsrfValidationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware.</param>
    public CsrfValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Processes the HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!ProtectedMethods.Contains(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.Value ?? string.Empty;

        if (IsIgnoredPath(path))
        {
            await _next(context);
            return;
        }

        if (!RequiresCsrfValidation(context))
        {
            await _next(context);
            return;
        }

        var cookieToken = context.Request.Cookies[CsrfCookieExtensions.CsrfCookieName];
        var headerToken = context.Request.Headers[CsrfCookieExtensions.CsrfHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(cookieToken) ||
            string.IsNullOrWhiteSpace(headerToken) ||
            !CryptographicEquals(cookieToken, headerToken))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            await context.Response.WriteAsJsonAsync(new
            {
                title = "Forbidden",
                status = StatusCodes.Status403Forbidden,
                detail = "Invalid or missing CSRF token.",
                traceId = context.TraceIdentifier
            });

            return;
        }

        await _next(context);
    }

    private static bool RequiresCsrfValidation(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            return true;
        }

        return context.Request.Cookies.ContainsKey("accessToken")
            || context.Request.Cookies.ContainsKey("refreshToken");
    }

    private static bool IsIgnoredPath(string path)
    {
        return path.StartsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("/api/auth/register-company", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("/api/auth/forgot-password", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("/api/auth/reset-password", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("/api/public/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool CryptographicEquals(string left, string right)
    {
        var leftBytes = System.Text.Encoding.UTF8.GetBytes(left);
        var rightBytes = System.Text.Encoding.UTF8.GetBytes(right);

        return leftBytes.Length == rightBytes.Length &&
               System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
