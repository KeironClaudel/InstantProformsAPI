using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Features.Proforms.Common;
using InstantProforms.Domain.Enums;

namespace InstantProforms.Application.Features.Proforms.SendProformByEmail;

/// <summary>
/// Handles sending a proform by email with a PDF attachment.
/// </summary>
public sealed class SendProformByEmailCommandHandler
    : IRequestHandler<SendProformByEmailCommand, SendProformByEmailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IProformPdfService _proformPdfService;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendProformByEmailCommandHandler"/> class.
    /// </summary>
    public SendProformByEmailCommandHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IProformPdfService proformPdfService,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _proformPdfService = proformPdfService;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    public async Task<SendProformByEmailResponse> Handle(
        SendProformByEmailCommand request,
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

        var pdfModel = ProformPdfModelFactory.Create(proform, settings);
        var pdfContent = await _proformPdfService.GenerateAsync(pdfModel, cancellationToken);

        var subject = string.IsNullOrWhiteSpace(request.Subject)
            ? $"Proform {proform.Number}"
            : request.Subject.Trim();

        var body = string.IsNullOrWhiteSpace(request.Message)
            ? $"""
               <p>Hello,</p>
               <p>Please find attached proform <strong>{proform.Number}</strong>.</p>
               <p>Thank you.</p>
               """
            : $"""
               <p>{System.Net.WebUtility.HtmlEncode(request.Message).Replace("\n", "<br />")}</p>
               <hr />
               <p>Attached proform: <strong>{proform.Number}</strong></p>
               """;

        await _emailService.SendAsync(
            request.ToEmail,
            subject,
            body,
            $"{proform.Number}.pdf",
            pdfContent,
            cancellationToken);

        if (proform.Status == ProformStatus.Draft)
        {
            proform.Status = ProformStatus.Sent;
            proform.UpdatedAtUtc = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new SendProformByEmailResponse(
            proform.Id,
            proform.Status.ToString(),
            "Proform email sent successfully.");
    }
}
