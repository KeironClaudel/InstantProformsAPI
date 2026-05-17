using Microsoft.AspNetCore.Http;

namespace InstantProforms.Application.Common.Files;

/// <summary>
/// Detects supported image formats using file signatures.
/// </summary>
public static class ImageFileInspector
{
    private static readonly SupportedImageFormat Png = new(".png", "image/png");
    private static readonly SupportedImageFormat Jpeg = new(".jpg", "image/jpeg");
    private static readonly SupportedImageFormat Webp = new(".webp", "image/webp");

    public static bool IsSupportedImage(IFormFile file)
    {
        return TryGetFormat(file, out _);
    }

    public static bool TryGetFormat(IFormFile file, out SupportedImageFormat? format)
    {
        ArgumentNullException.ThrowIfNull(file);

        using var stream = file.OpenReadStream();
        return TryGetFormat(stream, out format);
    }

    public static bool TryGetFormat(byte[] content, out SupportedImageFormat? format)
    {
        ArgumentNullException.ThrowIfNull(content);
        return TryGetFormat(content.AsSpan(), out format);
    }

    public static bool TryGetFormat(Stream stream, out SupportedImageFormat? format)
    {
        ArgumentNullException.ThrowIfNull(stream);

        Span<byte> header = stackalloc byte[12];
        var totalRead = 0;

        while (totalRead < header.Length)
        {
            var bytesRead = stream.Read(header[totalRead..]);
            if (bytesRead == 0)
            {
                break;
            }

            totalRead += bytesRead;
        }

        return TryGetFormat(header[..totalRead], out format);
    }

    public static bool HasExpectedExtension(string fileName, SupportedImageFormat format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(format);

        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(extension))
        {
            return false;
        }

        return format.MatchesExtension(extension);
    }

    private static bool TryGetFormat(ReadOnlySpan<byte> header, out SupportedImageFormat? format)
    {
        if (header.Length >= 8 &&
            header[0] == 0x89 &&
            header[1] == 0x50 &&
            header[2] == 0x4E &&
            header[3] == 0x47 &&
            header[4] == 0x0D &&
            header[5] == 0x0A &&
            header[6] == 0x1A &&
            header[7] == 0x0A)
        {
            format = Png;
            return true;
        }

        if (header.Length >= 3 &&
            header[0] == 0xFF &&
            header[1] == 0xD8 &&
            header[2] == 0xFF)
        {
            format = Jpeg;
            return true;
        }

        if (header.Length >= 12 &&
            header[0] == 0x52 &&
            header[1] == 0x49 &&
            header[2] == 0x46 &&
            header[3] == 0x46 &&
            header[8] == 0x57 &&
            header[9] == 0x45 &&
            header[10] == 0x42 &&
            header[11] == 0x50)
        {
            format = Webp;
            return true;
        }

        format = null;
        return false;
    }
}

/// <summary>
/// Represents a supported image format.
/// </summary>
public sealed record SupportedImageFormat(string Extension, string ContentType)
{
    public bool MatchesExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return false;
        }

        if (string.Equals(extension, Extension, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return string.Equals(Extension, ".jpg", StringComparison.OrdinalIgnoreCase) &&
               string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase);
    }
}
