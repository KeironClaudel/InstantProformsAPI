using FluentValidation;

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
            .MaximumLength(200);

        RuleFor(x => x)
            .Must(x => x.ClientId.HasValue || !string.IsNullOrWhiteSpace(x.ClientName))
            .WithMessage("Client name is required when no saved client is selected.");

        RuleFor(x => x.ClientEmail)
            .MaximumLength(200)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.ClientEmail));

        RuleFor(x => x.ClientPhone)
            .MaximumLength(50);

        RuleFor(x => x.Notes)
            .MaximumLength(1000);

        RuleFor(x => x.Location)
            .MaximumLength(1000);

        RuleFor(x => x.InternalNotes)
            .MaximumLength(4000);

        RuleFor(x => x.ClientIdentificationNumber)
            .MaximumLength(50);

        RuleFor(x => x.ServiceDescription)
            .MaximumLength(12000);

        RuleFor(x => x.ScopeOfWork)
            .MaximumLength(12000);

        RuleFor(x => x.ServiceConditions)
            .MaximumLength(12000);

        RuleFor(x => x.PaymentConditions)
            .MaximumLength(12000);

        RuleFor(x => x)
            .Must(x =>
                (x.ClientIdentificationType is null && string.IsNullOrWhiteSpace(x.ClientIdentificationNumber)) ||
                (x.ClientIdentificationType is not null && !string.IsNullOrWhiteSpace(x.ClientIdentificationNumber)))
            .WithMessage("Client identification type and number must be provided together.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one quotation line item is required.");

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
