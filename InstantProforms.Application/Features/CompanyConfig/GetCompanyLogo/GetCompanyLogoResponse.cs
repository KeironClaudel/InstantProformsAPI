namespace InstantProforms.Application.Features.CompanyConfig.GetCompanyLogo;

/// <summary>
/// Represents stored company logo content.
/// </summary>
public sealed record GetCompanyLogoResponse(
    byte[] Content,
    string ContentType,
    string FileName);
