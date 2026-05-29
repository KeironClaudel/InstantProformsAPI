using FluentValidation;
using InstantProforms.Domain.Enums;

namespace InstantProforms.Application.Features.Proforms.GetPagedProforms;

/// <summary>
/// Validates <see cref="GetPagedProformsQuery"/>.
/// </summary>
public sealed class GetPagedProformsQueryValidator : AbstractValidator<GetPagedProformsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPagedProformsQueryValidator"/> class.
    /// </summary>
    public GetPagedProformsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.ClientName)
            .MaximumLength(200);

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate!.Value)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

        RuleFor(x => x.Status)
            .Must(status => status is null || Enum.IsDefined(typeof(ProformStatus), status.Value))
            .WithMessage("A valid proform status is required when the status filter is provided.");
    }
}
