namespace InstantProforms.Api.Common.Helpers;

/// <summary>
/// Provides helper methods to build public file URLs.
/// </summary>
public static class FileUrlHelper
{
    /// <summary>
    /// Builds a public absolute URL for a stored relative file path.
    /// </summary>
    /// <param name="relativePath">The stored relative path.</param>
    /// <param name="scheme">The request scheme.</param>
    /// <param name="host">The request host.</param>
    /// <returns>The absolute file URL if the path is valid; otherwise, <c>null</c>.</returns>
    public static string? BuildAbsoluteUrl(string? relativePath, string scheme, HostString host)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        var normalizedPath = relativePath.Replace("\\", "/").TrimStart('/');

        return $"{scheme}://{host}/{normalizedPath}";
    }
}