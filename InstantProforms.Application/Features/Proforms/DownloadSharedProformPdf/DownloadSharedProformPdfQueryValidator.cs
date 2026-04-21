using FluentValidation;

namespace InstantProforms.Application.Features.Proforms.DownloadSharedProformPdf;

/// <summary>
/// Validates <see cref="DownloadSharedProformPdfQuery"/>.
/// </summary>
public sealed class DownloadSharedProformPdfQueryValidator : AbstractValidator<DownloadSharedProformPdfQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadSharedProformPdfQueryValidator"/> class.
    /// </summary>
    public DownloadSharedProformPdfQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}