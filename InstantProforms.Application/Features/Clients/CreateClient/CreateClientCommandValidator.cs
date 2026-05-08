using FluentValidation;

namespace InstantProforms.Application.Features.Clients.CreateClient;

/// <summary>
/// Validates <see cref="CreateClientCommand"/>.
/// </summary>
public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateClientCommandValidator"/> class.
    /// </summary>
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .MaximumLength(200)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(50);

        RuleFor(x => x.IdentificationNumber)
            .MaximumLength(50);

        RuleFor(x => x)
            .Must(x =>
                (x.IdentificationType is null && string.IsNullOrWhiteSpace(x.IdentificationNumber)) ||
                (x.IdentificationType is not null && !string.IsNullOrWhiteSpace(x.IdentificationNumber)))
            .WithMessage("Identification type and number must be provided together.");
    }
}
