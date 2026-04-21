using FluentValidation;

namespace InstantProforms.Application.Features.Proforms.SendProformByEmail;

/// <summary>
/// Validates <see cref="SendProformByEmailCommand"/>.
/// </summary>
public sealed class SendProformByEmailCommandValidator : AbstractValidator<SendProformByEmailCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SendProformByEmailCommandValidator"/> class.
    /// </summary>
    public SendProformByEmailCommandValidator()
    {
        RuleFor(x => x.ProformId)
            .NotEmpty();

        RuleFor(x => x.ToEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Subject)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Subject));

        RuleFor(x => x.Message)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Message));
    }
}