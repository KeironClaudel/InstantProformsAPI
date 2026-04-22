using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Features.Proforms.Common;

namespace InstantProforms.Application.Features.Proforms.DownloadProformPdf;

/// <summary>
/// Handles generation of a proform PDF.
/// </summary>
public sealed class DownloadProformPdfQueryHandler
    : IRequestHandler<DownloadProformPdfQuery, DownloadProformPdfResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProformPdfService _proformPdfService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadProformPdfQueryHandler"/> class.
    /// </summary>
    public DownloadProformPdfQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IProformPdfService proformPdfService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _proformPdfService = proformPdfService;
    }

    /// <inheritdoc />
    public async Task<DownloadProformPdfResponse> Handle(
        DownloadProformPdfQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var companyId = _currentUserService.CompanyId.Value;

        var proform = await _unitOfWork.Proforms
            .GetByIdWithItemsAsync(request.ProformId, companyId, cancellationToken);

        if (proform is null)
        {
            throw new InvalidOperationException("Proform was not found.");
        }

        var settings = await _unitOfWork.CompanySettings
            .GetByCompanyIdAsync(companyId, cancellationToken);

        if (settings is null)
        {
            throw new InvalidOperationException("Company settings were not found.");
        }

        var model = ProformPdfModelFactory.Create(proform, settings);
        var content = _proformPdfService.Generate(model);

        return new DownloadProformPdfResponse(
            content,
            $"{proform.Number}.pdf",
            "application/pdf");
    }
}