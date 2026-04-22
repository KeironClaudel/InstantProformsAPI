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
    Task DeleteAsync(string relativePath);
}

/// <summary>
/// Represents the result of a file save operation.
/// </summary>
public sealed record FileStorageSaveResult(
    string StoredFileName,
    string RelativePath);