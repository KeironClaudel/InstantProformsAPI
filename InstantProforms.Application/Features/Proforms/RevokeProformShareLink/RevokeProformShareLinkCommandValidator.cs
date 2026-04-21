using FluentValidation;

namespace InstantProforms.Application.Features.Proforms.RevokeProformShareLink;

/// <summary>
/// Validates <see cref="RevokeProformShareLinkCommand"/>.
/// </summary>
public sealed class RevokeProformShareLinkCommandValidator : AbstractValidator<RevokeProformShareLinkCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeProformShareLinkCommandValidator"/> class.
    /// </summary>
    public RevokeProformShareLinkCommandValidator()
    {
        RuleFor(x => x.ProformId)
            .NotEmpty();

        RuleFor(x => x.ShareTokenId)
            .NotEmpty();
    }
}