using FluentValidation;
using InstantProforms.Application.Features.Proforms.CreateProform;

namespace InstantProforms.Application.Features.Proforms.CreateProform;

/// <summary>
/// Validates <see cref="CreateProformCommand"/>.
/// </summary>
public sealed class CreateProformCommandValidator : AbstractValidator<CreateProformCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProformCommandValidator"/> class.
    /// </summary>
    public CreateProformCommandValidator()
    {
        RuleFor(x => x.ClientName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ClientEmail)
            .MaximumLength(200)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.ClientEmail));

        RuleFor(x => x.ClientPhone)
            .MaximumLength(50);

        RuleFor(x => x.Notes)
            .MaximumLength(1000);

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one proforms item is required.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateProformItemModelValidator());
    }
}

internal sealed class CreateProformItemModelValidator : AbstractValidator<CreateProformItemModel>
{
    public CreateProformItemModelValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0);
    }
}