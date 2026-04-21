using FluentValidation;

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
    }
}