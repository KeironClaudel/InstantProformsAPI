using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Features.Proforms.Common;

namespace InstantProforms.Application.Features.Proforms.DownloadSharedProformPdf;

/// <summary>
/// Handles generation of a shared proform PDF using a temporary token.
/// </summary>
public sealed class DownloadSharedProformPdfQueryHandler
    : IRequestHandler<DownloadSharedProformPdfQuery, DownloadSharedProformPdfResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenHashService _tokenHashService;
    private readonly IProformPdfService _proformPdfService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadSharedProformPdfQueryHandler"/> class.
    /// </summary>
    public DownloadSharedProformPdfQueryHandler(
        IUnitOfWork unitOfWork,
        ITokenHashService tokenHashService,
        IProformPdfService proformPdfService)
    {
        _unitOfWork = unitOfWork;
        _tokenHashService = tokenHashService;
        _proformPdfService = proformPdfService;
    }

    /// <inheritdoc />
    public async Task<DownloadSharedProformPdfResponse> Handle(
        DownloadSharedProformPdfQuery request,
        CancellationToken cancellationToken)
    {
        var tokenHash = _tokenHashService.ComputeHash(request.Token);

        var shareToken = await _unitOfWork.ProformShareTokens
            .GetByTokenHashWithProformAsync(tokenHash, cancellationToken);

        if (shareToken is null || !shareToken.IsActive)
        {
            throw new InvalidOperationException("Invalid or expired share token.");
        }

        if (shareToken.IsSingleUse)
        {
            shareToken.UsedAtUtc = DateTime.UtcNow;
        }

        var proform = shareToken.Proform;

        var settings = await _unitOfWork.CompanySettings
            .GetByCompanyIdAsync(proform.CompanyId, cancellationToken);

        if (settings is null)
        {
            throw new InvalidOperationException("Company settings were not found.");
        }

        var model = ProformPdfModelFactory.Create(proform, settings);
        var content = await _proformPdfService.GenerateAsync(model, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DownloadSharedProformPdfResponse(
            content,
            $"{proform.Number}.pdf",
            "application/pdf");
    }
}
