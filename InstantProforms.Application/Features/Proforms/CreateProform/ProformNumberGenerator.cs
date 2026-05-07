using System.Text.RegularExpressions;

namespace InstantProforms.Application.Features.Proforms.CreateProform;

/// <summary>
/// Generates yearly proform numbers using a year-based letter prefix.
/// </summary>
public static partial class ProformNumberGenerator
{
    private const int BaseYear = 2024;
    private const int StartingSequence = 200;

    /// <summary>
    /// Generates the next proform number for the specified year.
    /// </summary>
    /// <param name="latestNumber">The latest proform number for the company.</param>
    /// <param name="year">The year used to generate the sequence prefix.</param>
    /// <returns>The next proform number.</returns>
    public static string GenerateNextNumber(string? latestNumber, int year)
    {
        var prefix = BuildYearPrefix(year);

        if (string.IsNullOrWhiteSpace(latestNumber))
        {
            return $"{prefix}{StartingSequence}";
        }

        var match = ProformNumberPattern().Match(latestNumber.Trim().ToUpperInvariant());

        if (!match.Success)
        {
            return $"{prefix}{StartingSequence}";
        }

        var latestPrefix = match.Groups["prefix"].Value;
        var latestYear = int.Parse(match.Groups["year"].Value);
        var latestSequence = int.Parse(match.Groups["sequence"].Value);

        if (!string.Equals(latestPrefix, GetYearCode(year), StringComparison.Ordinal)
            || latestYear != year)
        {
            return $"{prefix}{StartingSequence}";
        }

        return $"{prefix}{latestSequence + 1}";
    }

    private static string BuildYearPrefix(int year)
    {
        return $"{GetYearCode(year)}{year}";
    }

    private static string GetYearCode(int year)
    {
        var offset = Math.Max(0, year - BaseYear);
        var code = string.Empty;

        do
        {
            code = (char)('A' + (offset % 26)) + code;
            offset = (offset / 26) - 1;
        }
        while (offset >= 0);

        return code;
    }

    [GeneratedRegex(@"^(?<prefix>[A-Z]+)(?<year>\d{4})(?<sequence>\d+)$", RegexOptions.Compiled)]
    private static partial Regex ProformNumberPattern();
}
