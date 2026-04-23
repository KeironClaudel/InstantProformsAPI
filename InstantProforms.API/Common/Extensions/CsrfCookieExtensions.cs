namespace InstantProforms.Api.Common.Extensions;

/// <summary>
/// Provides CSRF cookie constants shared across the API.
/// </summary>
public static class CsrfCookieExtensions
{
    public const string CsrfCookieName = "XSRF-TOKEN";
    public const string CsrfHeaderName = "X-CSRF-TOKEN";
}
