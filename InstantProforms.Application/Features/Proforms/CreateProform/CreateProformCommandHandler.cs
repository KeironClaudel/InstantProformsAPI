using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;
using InstantProforms.Domain.Enums;
using MediatR;

namespace InstantProforms.Application.Features.Proforms.CreateProform;

/// <summary>
/// Handles proforms creation.
/// </summary>
public sealed class CreateProformCommandHandler
    : IRequestHandler<CreateProformCommand, CreateProformResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProformCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="currentUserService">The current user service.</param>
    public CreateProformCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    public async Task<CreateProformResponse> Handle(
        CreateProformCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var companyId = _currentUserService.CompanyId.Value;
        Client? selectedClient = null;

        if (request.ClientId.HasValue)
        {
            selectedClient = await _unitOfWork.Clients.GetByIdAsync(
                request.ClientId.Value,
                companyId,
                cancellationToken);

            if (selectedClient is null)
            {
                throw new InvalidOperationException("The selected client was not found.");
            }
        }

        var settings = await _unitOfWork.CompanySettings
            .GetByCompanyIdAsync(companyId, cancellationToken);

        if (settings is null)
        {
            throw new InvalidOperationException("Company settings were not found.");
        }

        var utcNow = DateTime.UtcNow;
        var latestProform = await _unitOfWork.Proforms
            .GetLatestByCompanyAsync(companyId, cancellationToken);

        var nextNumber = ProformNumberGenerator.GenerateNextNumber(latestProform?.Number, utcNow.Year);
        var proformId = Guid.NewGuid();

        var items = request.Items
            .Select((item, index) =>
            {
                var lineTotal = decimal.Round(item.Quantity * item.UnitPrice, 2);

                return new ProformItem
                {
                    Id = Guid.NewGuid(),
                    ProformId = proformId,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = lineTotal,
                    SortOrder = index + 1,
                    CreatedAtUtc = utcNow
                };
            })
            .ToList();

        var subtotal = items.Sum(x => x.Total);
        var taxPercentage = settings.TaxPercentage;
        var taxAmount = decimal.Round(subtotal * (taxPercentage / 100m), 2);
        var total = subtotal + taxAmount;

        var clientName = string.IsNullOrWhiteSpace(request.ClientName) ? selectedClient?.Name : request.ClientName.Trim();
        var clientEmail = string.IsNullOrWhiteSpace(request.ClientEmail) ? selectedClient?.Email : request.ClientEmail.Trim();
        var clientPhone = string.IsNullOrWhiteSpace(request.ClientPhone) ? selectedClient?.Phone : request.ClientPhone.Trim();
        var clientIdentificationType = request.ClientIdentificationType ?? selectedClient?.IdentificationType;
        var clientIdentificationNumber = string.IsNullOrWhiteSpace(request.ClientIdentificationNumber)
            ? selectedClient?.IdentificationNumber
            : request.ClientIdentificationNumber.Trim();
        var resolvedLocation = !string.IsNullOrWhiteSpace(request.Location)
            ? request.Location.Trim()
            : string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        var resolvedCurrency = request.Currency ?? ProformCurrency.Colones;

        var proform = new Proform
        {
            Id = proformId,
            CompanyId = companyId,
            Number = nextNumber,
            ClientId = selectedClient?.Id,
            Currency = resolvedCurrency,
            Status = ProformStatus.Draft,
            ClientName = clientName ?? string.Empty,
            ClientEmail = clientEmail,
            ClientPhone = clientPhone,
            ClientIdentificationType = clientIdentificationType,
            ClientIdentificationNumber = clientIdentificationNumber,
            Location = resolvedLocation,
            InternalNotes = string.IsNullOrWhiteSpace(request.InternalNotes) ? null : request.InternalNotes.Trim(),
            ServiceDescription = string.IsNullOrWhiteSpace(request.ServiceDescription) ? null : request.ServiceDescription.Trim(),
            ScopeOfWork = string.IsNullOrWhiteSpace(request.ScopeOfWork) ? null : request.ScopeOfWork.Trim(),
            ServiceConditions = string.IsNullOrWhiteSpace(request.ServiceConditions) ? null : request.ServiceConditions.Trim(),
            PaymentConditions = string.IsNullOrWhiteSpace(request.PaymentConditions) ? null : request.PaymentConditions.Trim(),
            IssuedAtUtc = utcNow,
            Subtotal = subtotal,
            TaxPercentage = taxPercentage,
            TaxAmount = taxAmount,
            Total = total,
            CreatedAtUtc = utcNow,
            Items = items
        };

        await _unitOfWork.Proforms.AddAsync(proform, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProformResponse(
            proform.Id,
            proform.Number,
            proform.Currency.ToString(),
            proform.Status.ToString(),
            proform.Subtotal,
            proform.TaxPercentage,
            proform.TaxAmount,
            proform.Total);
    }
}
