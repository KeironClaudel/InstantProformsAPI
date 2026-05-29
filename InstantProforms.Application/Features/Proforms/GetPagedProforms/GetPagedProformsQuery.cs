using InstantProforms.Domain.Enums;
using MediatR;

namespace InstantProforms.Application.Features.Proforms.GetPagedProforms;

/// <summary>
/// Represents a request to get paginated Proforms.
/// </summary>
public sealed record GetPagedProformsQuery(
    int Page = 1,
    int PageSize = 10,
    string? ClientName = null,
    ProformStatus? Status = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null) : IRequest<GetPagedProformsResponse>;
