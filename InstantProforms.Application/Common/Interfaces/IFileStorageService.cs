namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines file storage operations.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a company logo file and returns its relative storage path and stored file name.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="fileName">The original file name.</param>
    /// <param name="content">The file content stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stored file result.</returns>
    Task<FileStorageSaveResult> SaveCompanyLogoAsync(
        Guid companyId,
        string fileName,
        Stream content,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a stored file if it exists.
    /// </summary>
    /// <param name="relativePath">The relative storage path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAsync(string relativePath, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the public URL for a stored file when available.
    /// </summary>
    /// <param name="relativePath">The relative storage path.</param>
    /// <returns>The public URL or <c>null</c> when no path is provided.</returns>
    string? GetPublicUrl(string? relativePath);

    /// <summary>
    /// Gets the stored file content as bytes.
    /// </summary>
    /// <param name="relativePath">The relative storage path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The file content or <c>null</c> when it cannot be retrieved.</returns>
    Task<byte[]?> GetBytesAsync(string? relativePath, CancellationToken cancellationToken);
}

/// <summary>
/// Represents the result of a file save operation.
/// </summary>
public sealed record FileStorageSaveResult(
    string StoredFileName,
    string RelativePath);
