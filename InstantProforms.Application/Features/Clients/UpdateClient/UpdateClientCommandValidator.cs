using FluentValidation;

namespace InstantProforms.Application.Features.Clients.UpdateClient;

/// <summary>
/// Validates <see cref="UpdateClientCommand"/>.
/// </summary>
public sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateClientCommandValidator"/> class.
    /// </summary>
    public UpdateClientCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty();

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
