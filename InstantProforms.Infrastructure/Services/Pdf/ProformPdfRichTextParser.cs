using System.Text.RegularExpressions;

namespace InstantProforms.Infrastructure.Services.Pdf;

/// <summary>
/// Parses plain text content into paragraph and bullet blocks for PDF rendering.
/// </summary>
public static class ProformPdfRichTextParser
{
    private static readonly Regex NumberedBulletRegex = new(@"^\d+[\.\)]\s+", RegexOptions.Compiled);
    private static readonly Regex SentenceSplitRegex = new(@"(?<=[\.;:])\s+(?=[A-ZÁÉÍÓÚÑ])", RegexOptions.Compiled);

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

        var blocks = new List<ProformPdfTextBlock>();
        TextBlockBuilder? currentBlock = null;

        foreach (var rawLine in normalized.Split('\n'))
        {
            var line = NormalizeWhitespace(rawLine);

            if (string.IsNullOrWhiteSpace(line))
            {
                FlushCurrentBlock();
                continue;
            }

            if (TryExtractBulletText(line, out var bulletText))
            {
                FlushCurrentBlock();
                currentBlock = new TextBlockBuilder(ProformPdfTextBlockKind.Bullet, bulletText);
                continue;
            }

            currentBlock ??= new TextBlockBuilder(ProformPdfTextBlockKind.Paragraph, line);
            currentBlock.Append(line);
        }

        FlushCurrentBlock();

        return blocks;

        void FlushCurrentBlock()
        {
            if (currentBlock is null)
            {
                return;
            }

            var text = currentBlock.Build();
            if (!string.IsNullOrWhiteSpace(text))
            {
                blocks.Add(new ProformPdfTextBlock(currentBlock.Kind, text));
            }

            currentBlock = null;
        }
    }

    /// <summary>
    /// Parses general conditions trying to keep one condition per rendered row.
    /// </summary>
    /// <param name="value">The source text.</param>
    /// <returns>The parsed condition blocks.</returns>
    public static IReadOnlyList<ProformPdfTextBlock> ParseConditions(string? value)
    {
        var normalized = NormalizeInput(value);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Array.Empty<ProformPdfTextBlock>();
        }

        if (normalized.Contains("\n\n", StringComparison.Ordinal))
        {
            return normalized
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(NormalizeWhitespace)
                .Where(static x => !string.IsNullOrWhiteSpace(x))
                .Select(static x => new ProformPdfTextBlock(ProformPdfTextBlockKind.Bullet, x))
                .ToList();
        }

        if (normalized.Contains('\n'))
        {
            return normalized
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(NormalizeWhitespace)
                .Where(static x => !string.IsNullOrWhiteSpace(x))
                .Select(static x => new ProformPdfTextBlock(ProformPdfTextBlockKind.Bullet, x))
                .ToList();
        }

        var sentenceBlocks = SentenceSplitRegex
            .Split(normalized)
            .Select(NormalizeWhitespace)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        if (sentenceBlocks.Count > 1)
        {
            return sentenceBlocks
                .Select(static x => new ProformPdfTextBlock(ProformPdfTextBlockKind.Bullet, x))
                .ToList();
        }

        return new[]
        {
            new ProformPdfTextBlock(ProformPdfTextBlockKind.Bullet, NormalizeWhitespace(normalized))
        };
    }

    /// <summary>
    /// Parses the service conditions section by preserving free-form user content
    /// and appending company default conditions using the legacy one-row-per-condition format.
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

        if (value.Length > 2 && (value.StartsWith("- ") || value.StartsWith("* ") || value.StartsWith("• ") || value.StartsWith("· ")))
        {
            bulletText = value[2..].Trim();
            return true;
        }

        var numberedMatch = NumberedBulletRegex.Match(value);
        if (numberedMatch.Success)
        {
            bulletText = value[numberedMatch.Length..].Trim();
            return true;
        }

        return false;
    }

    private static string NormalizeWhitespace(string value)
    {
        return Regex.Replace(value.Trim(), @"\s+", " ");
    }

    private sealed class TextBlockBuilder
    {
        private readonly List<string> _segments;

        public TextBlockBuilder(ProformPdfTextBlockKind kind, string initialValue)
        {
            Kind = kind;
            _segments = new List<string> { initialValue };
        }

        public ProformPdfTextBlockKind Kind { get; }

        public void Append(string value)
        {
            if (_segments.Count == 0 || !string.Equals(_segments[^1], value, StringComparison.Ordinal))
            {
                _segments.Add(value);
            }
        }

        public string Build() => string.Join(" ", _segments);
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
