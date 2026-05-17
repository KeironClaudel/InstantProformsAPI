using FluentValidation;
using InstantProforms.Application.Common.Files;

namespace InstantProforms.Application.Features.CompanyConfig.ReplaceLogo;

/// <summary>
/// Validates <see cref="ReplaceCompanyLogoCommand"/>.
/// </summary>
public sealed class ReplaceCompanyLogoCommandValidator : AbstractValidator<ReplaceCompanyLogoCommand>
{
    public ReplaceCompanyLogoCommandValidator()
    {
        RuleFor(x => x.LogoFile)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Logo file is required.")
            .Must(x => x.Length <= 2 * 1024 * 1024)
            .WithMessage("Max size is 2MB.")
            .Must(file =>
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                return ext is ".png" or ".jpg" or ".jpeg" or ".webp";
            })
            .WithMessage("Invalid file type.")
            .Must(file =>
            {
                if (!ImageFileInspector.TryGetFormat(file, out var format) || format is null)
                {
                    return false;
                }

                return ImageFileInspector.HasExpectedExtension(file.FileName, format);
            })
            .WithMessage("The uploaded file content does not match a supported image format.");
    }
}
