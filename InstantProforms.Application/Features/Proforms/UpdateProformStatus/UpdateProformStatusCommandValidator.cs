using FluentValidation;

namespace InstantProforms.Application.Features.Proforms.UpdateProformStatus;

/// <summary>
/// Validates <see cref="UpdateProformStatusCommand"/>.
/// </summary>
public sealed class UpdateProformStatusCommandValidator : AbstractValidator<UpdateProformStatusCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProformStatusCommandValidator"/> class.
    /// </summary>
    public UpdateProformStatusCommandValidator()
    {
        RuleFor(x => x.ProformId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(50);
    }
}