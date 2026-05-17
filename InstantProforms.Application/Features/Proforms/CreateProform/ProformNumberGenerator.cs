using System.Text.RegularExpressions;

namespace InstantProforms.Application.Features.Proforms.CreateProform;

/// <summary>
/// Generates yearly proform numbers using a year-based letter prefix.
/// </summary>
public static partial class ProformNumberGenerator
{
    private const int StartingSequence = 200;

    /// <summary>
    /// Generates the next proform number for the specified year.
    /// </summary>
    /// <param name="latestNumber">The latest proform number for the company.</param>
    /// <param name="basePrefix">The configured prefix anchor for the company's base year.</param>
    /// <param name="baseYear">The year in which the configured prefix applies.</param>
    /// <param name="year">The year used to generate the sequence prefix.</param>
    /// <returns>The next proform number.</returns>
    public static string GenerateNextNumber(string? latestNumber, string basePrefix, int baseYear, int year)
    {
        var prefix = BuildYearPrefix(basePrefix, baseYear, year);

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

        if (!string.Equals(latestPrefix, GetYearCode(basePrefix, baseYear, year), StringComparison.Ordinal)
            || latestYear != year)
        {
            return $"{prefix}{StartingSequence}";
        }

        return $"{prefix}{latestSequence + 1}";
    }

    private static string BuildYearPrefix(string basePrefix, int baseYear, int year)
    {
        return $"{GetYearCode(basePrefix, baseYear, year)}{year}";
    }

    private static string GetYearCode(string basePrefix, int baseYear, int year)
    {
        var normalizedPrefix = NormalizeBasePrefix(basePrefix);
        var offset = Math.Max(0, year - baseYear);
        var codeChars = normalizedPrefix.ToCharArray();

        while (offset > 0)
        {
            IncrementCode(codeChars);
            offset--;
        }

        return new string(codeChars);
    }

    private static string NormalizeBasePrefix(string basePrefix)
    {
        var lettersOnly = new string(basePrefix
            .Trim()
            .ToUpperInvariant()
            .Where(char.IsLetter)
            .ToArray());

        return string.IsNullOrWhiteSpace(lettersOnly)
            ? "A"
            : lettersOnly;
    }

    private static void IncrementCode(IList<char> codeChars)
    {
        for (var index = codeChars.Count - 1; index >= 0; index--)
        {
            if (codeChars[index] < 'Z')
            {
                codeChars[index]++;
                return;
            }

            codeChars[index] = 'A';
        }

        codeChars.Insert(0, 'A');
    }

    [GeneratedRegex(@"^(?<prefix>[A-Z]+)(?<year>\d{4})(?<sequence>\d+)$", RegexOptions.Compiled)]
    private static partial Regex ProformNumberPattern();
}
