using MediatR;

namespace InstantProforms.Application.Features.CompanyConfig.GetCompanySettings;

/// <summary>
/// Represents a request to get the current company settings.
/// </summary>
public sealed record GetCompanySettingsQuery : IRequest<GetCompanySettingsResponse>;