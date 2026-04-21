using FluentValidation;

namespace InstantProforms.Application.Features.Proforms.CreateProformShareLink;

/// <summary>
/// Validates <see cref="CreateProformShareLinkCommand"/>.
/// </summary>
public sealed class CreateProformShareLinkCommandValidator : AbstractValidator<CreateProformShareLinkCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProformShareLinkCommandValidator"/> class.
    /// </summary>
    public CreateProformShareLinkCommandValidator()
    {
        RuleFor(x => x.ProformId)
            .NotEmpty();

        RuleFor(x => x.ExpirationMinutes)
            .InclusiveBetween(1, 1440)
            .When(x => x.ExpirationMinutes.HasValue);
    }
}