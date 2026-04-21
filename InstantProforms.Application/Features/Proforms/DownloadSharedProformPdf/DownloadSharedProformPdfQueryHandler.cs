using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Features.Proforms.GetProformById;

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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var proform = shareToken.Proform;

        var response = new GetProformByIdResponse(
            proform.Id,
            proform.Number,
            proform.Status.ToString(),
            proform.ClientName,
            proform.ClientEmail,
            proform.ClientPhone,
            proform.IssuedAtUtc,
            proform.Notes,
            proform.Subtotal,
            proform.Total,
            proform.Items
                .OrderBy(x => x.SortOrder)
                .Select(x => new GetProformByIdItemResponse(
                    x.Id,
                    x.Description,
                    x.Quantity,
                    x.UnitPrice,
                    x.Total,
                    x.SortOrder))
                .ToList());

        var content = _proformPdfService.Generate(response);

        return new DownloadSharedProformPdfResponse(
            content,
            $"{proform.Number}.pdf",
            "application/pdf");
    }
}