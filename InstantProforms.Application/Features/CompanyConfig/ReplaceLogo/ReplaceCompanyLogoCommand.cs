using MediatR;
using Microsoft.AspNetCore.Http;

namespace InstantProforms.Application.Features.CompanyConfig.ReplaceLogo;

/// <summary>
/// Represents a request to replace the company logo.
/// </summary>
public sealed record ReplaceCompanyLogoCommand(
    IFormFile LogoFile) : IRequest;