namespace InstantProforms.Application.Common.Security;

/// <summary>
/// Normalizes sensitive user-provided values before protection or lookup.
/// </summary>
public static class SensitiveValueNormalizer
{
    /// <summary>
    /// Normalizes an identification number while preserving user-visible formatting.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The normalized value or <c>null</c>.</returns>
    public static string? NormalizeIdentificationNumber(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
