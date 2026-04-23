
namespace InstantProforms.Api.Contracts.CompanySettings;

/// <summary>
/// Represents the HTTP request to replace the company logo.
/// </summary>
public sealed class ReplaceCompanyLogoRequest
{
    /// <summary>
    /// Gets or sets the logo file.
    /// </summary>
    public IFormFile LogoFile { get; set; } = null!;
}