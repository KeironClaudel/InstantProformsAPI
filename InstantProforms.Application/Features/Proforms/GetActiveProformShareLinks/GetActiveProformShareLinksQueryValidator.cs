using FluentValidation;

namespace InstantProforms.Application.Features.Proforms.GetActiveProformShareLinks;

/// <summary>
/// Validates <see cref="GetActiveProformShareLinksQuery"/>.
/// </summary>
public sealed class GetActiveProformShareLinksQueryValidator : AbstractValidator<GetActiveProformShareLinksQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetActiveProformShareLinksQueryValidator"/> class.
    /// </summary>
    public GetActiveProformShareLinksQueryValidator()
    {
        RuleFor(x => x.ProformId)
            .NotEmpty();
    }
}