using FluentValidation;

namespace InstantProforms.Application.Features.Auth.RegisterCompany;

public sealed class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
{
    public RegisterCompanyCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CompanySlug)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Company slug must contain only lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.CompanyEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.OwnerFullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.CompanyWebsite)
            .MaximumLength(200)
            .WithMessage("Company website must be a valid URL.");

        RuleFor(x => x.OwnerEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
    }
}