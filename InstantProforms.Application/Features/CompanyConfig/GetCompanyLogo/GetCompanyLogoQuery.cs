using MediatR;

namespace InstantProforms.Application.Features.CompanyConfig.GetCompanyLogo;

/// <summary>
/// Represents a request to get the current company logo bytes.
/// </summary>
public sealed record GetCompanyLogoQuery : IRequest<GetCompanyLogoResponse?>;
