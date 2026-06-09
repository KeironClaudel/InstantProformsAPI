using System.Text.RegularExpressions;

namespace InstantProforms.Infrastructure.Services.Pdf;

/// <summary>
/// Parses plain text content into paragraph and bullet blocks for PDF rendering.
/// </summary>
public static class ProformPdfRichTextParser
{
    /// <summary>
    /// Parses the provided value into rich text blocks.
    /// </summary>
    /// <param name="value">The source text.</param>
    /// <returns>The parsed blocks.</returns>
    public static IReadOnlyList<ProformPdfTextBlock> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<ProformPdfTextBlock>();
        }

        var normalized = NormalizeInput(value);
        return normalized
            .Split('\n')
            .Select(ParseLine)
            .Where(static block => block is not null)
            .Select(static block => block!)
            .ToList();
    }

    /// <summary>
    /// Parses general conditions using the same modern formatting rules as the rest of the document.
    /// </summary>
    /// <param name="value">The source text.</param>
    /// <returns>The parsed condition blocks.</returns>
    public static IReadOnlyList<ProformPdfTextBlock> ParseConditions(string? value)
    {
        return Parse(value);
    }

    /// <summary>
    /// Parses the service conditions section by preserving free-form user content
    /// and appending company default conditions with the same modern formatting rules.
    /// </summary>
    /// <param name="userValue">The user-authored service conditions.</param>
    /// <param name="defaultConditionsValue">The company default conditions.</param>
    /// <returns>The combined section blocks.</returns>
    public static IReadOnlyList<ProformPdfTextBlock> ParseServiceConditions(
        string? userValue,
        string? defaultConditionsValue)
    {
        var blocks = new List<ProformPdfTextBlock>();

        blocks.AddRange(Parse(userValue));
        blocks.AddRange(ParseConditions(defaultConditionsValue));

        return blocks;
    }

    private static string NormalizeInput(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value
                .Replace("\\n", "\n")
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Trim();
    }

    private static bool TryExtractBulletText(string value, out string bulletText)
    {
        bulletText = value;

        if (value.Length > 2 && value.StartsWith("- ", StringComparison.Ordinal))
        {
            bulletText = value[2..].Trim();
            return true;
        }

        return false;
    }

    private static ProformPdfTextBlock? ParseLine(string rawLine)
    {
        var line = NormalizeWhitespace(rawLine);
        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }

        if (TryExtractBulletText(line, out var bulletText))
        {
            return new ProformPdfTextBlock(ProformPdfTextBlockKind.Bullet, bulletText);
        }

        return new ProformPdfTextBlock(ProformPdfTextBlockKind.Paragraph, line);
    }

    private static string NormalizeWhitespace(string value)
    {
        return Regex.Replace(value.Trim(), @"\s+", " ");
    }
}

/// <summary>
/// Represents a parsed text block for PDF sections.
/// </summary>
/// <param name="Kind">The block kind.</param>
/// <param name="Text">The block text.</param>
public sealed record ProformPdfTextBlock(ProformPdfTextBlockKind Kind, string Text);

/// <summary>
/// Describes the type of parsed text block.
/// </summary>
public enum ProformPdfTextBlockKind
{
    /// <summary>
    /// A regular paragraph.
    /// </summary>
    Paragraph = 0,

    /// <summary>
    /// A bullet list item.
    /// </summary>
    Bullet = 1
}
