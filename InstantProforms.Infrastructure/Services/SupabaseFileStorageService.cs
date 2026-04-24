using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Stores company assets in Supabase Storage.
/// </summary>
public sealed class SupabaseFileStorageService : IFileStorageService
{
    private static readonly IReadOnlyDictionary<string, string> ContentTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".webp"] = "image/webp",
        [".gif"] = "image/gif",
        [".svg"] = "image/svg+xml"
    };

    private readonly HttpClient _httpClient;
    private readonly SupabaseStorageSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SupabaseFileStorageService"/> class.
    /// </summary>
    public SupabaseFileStorageService(
        HttpClient httpClient,
        IOptions<SupabaseStorageSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;

        if (string.IsNullOrWhiteSpace(_settings.Url))
        {
            throw new InvalidOperationException("SupabaseStorage:Url is required.");
        }

        if (string.IsNullOrWhiteSpace(_settings.ServiceRoleKey))
        {
            throw new InvalidOperationException("SupabaseStorage:ServiceRoleKey is required.");
        }

        if (string.IsNullOrWhiteSpace(_settings.BucketName))
        {
            throw new InvalidOperationException("SupabaseStorage:BucketName is required.");
        }

        _httpClient.BaseAddress = new Uri(_settings.Url.TrimEnd('/') + "/", UriKind.Absolute);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ServiceRoleKey);
        _httpClient.DefaultRequestHeaders.Remove("apikey");
        _httpClient.DefaultRequestHeaders.Add("apikey", _settings.ServiceRoleKey);
    }

    /// <inheritdoc />
    public async Task<FileStorageSaveResult> SaveCompanyLogoAsync(
        Guid companyId,
        string fileName,
        Stream content,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var relativePath = BuildRelativePath(companyId, storedFileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, BuildObjectUri(relativePath));
        request.Headers.TryAddWithoutValidation("x-upsert", "true");

        var streamContent = new StreamContent(content);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(extension));
        request.Content = streamContent;

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return new FileStorageSaveResult(storedFileName, relativePath);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string relativePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        using var response = await _httpClient.DeleteAsync(BuildObjectUri(relativePath), cancellationToken);

        if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public string? GetPublicUrl(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        if (Uri.TryCreate(relativePath, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }

        return new Uri(_httpClient.BaseAddress!, BuildPublicObjectUri(relativePath)).ToString();
    }

    /// <inheritdoc />
    public async Task<byte[]?> GetBytesAsync(string? relativePath, CancellationToken cancellationToken)
    {
        var publicUrl = GetPublicUrl(relativePath);

        if (string.IsNullOrWhiteSpace(publicUrl))
        {
            return null;
        }

        using var response = await _httpClient.GetAsync(publicUrl, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    private string BuildRelativePath(Guid companyId, string storedFileName)
    {
        var companyLogosFolder = _settings.CompanyLogosFolder.Trim('/').Replace("\\", "/");
        return $"{companyLogosFolder}/{companyId}/{storedFileName}";
    }

    private string BuildObjectUri(string relativePath)
    {
        return $"storage/v1/object/{EscapePathSegment(_settings.BucketName)}/{EscapeObjectPath(relativePath)}";
    }

    private string BuildPublicObjectUri(string relativePath)
    {
        return $"storage/v1/object/public/{EscapePathSegment(_settings.BucketName)}/{EscapeObjectPath(relativePath)}";
    }

    private static string EscapeObjectPath(string relativePath)
    {
        return string.Join(
            "/",
            relativePath
                .Replace("\\", "/")
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(EscapePathSegment));
    }

    private static string EscapePathSegment(string segment)
    {
        return Uri.EscapeDataString(segment);
    }

    private static string GetContentType(string extension)
    {
        return ContentTypes.TryGetValue(extension, out var contentType)
            ? contentType
            : "application/octet-stream";
    }
}
