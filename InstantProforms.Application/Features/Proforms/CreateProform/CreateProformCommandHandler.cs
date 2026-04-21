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

        var latestproforms = await _unitOfWork.Proforms
            .GetLatestByCompanyAsync(companyId, cancellationToken);

        var nextNumber = GenerateNextNumber(latestproforms?.Number);

        var utcNow = DateTime.UtcNow;
        var proformsId = Guid.NewGuid();

        var items = request.Items
            .Select((item, index) =>
            {
                var lineTotal = decimal.Round(item.Quantity * item.UnitPrice, 2);

                return new ProformItem
                {
                    Id = Guid.NewGuid(),
                    proformsId = proformsId,
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
        var total = subtotal;

        var proforms = new Proform
        {
            Id = proformsId,
            CompanyId = companyId,
            Number = nextNumber,
            Status = ProformStatus.Draft,
            ClientName = request.ClientName,
            ClientEmail = request.ClientEmail,
            ClientPhone = request.ClientPhone,
            Notes = request.Notes,
            IssuedAtUtc = utcNow,
            Subtotal = subtotal,
            Total = total,
            CreatedAtUtc = utcNow,
            Items = items
        };

        await _unitOfWork.Proforms.AddAsync(proforms, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProformResponse(
            proforms.Id,
            proforms.Number,
            proforms.Status.ToString(),
            proforms.Subtotal,
            proforms.Total);
    }

    private static string GenerateNextNumber(string? latestNumber)
    {
        const string prefix = "PRO-";

        if (string.IsNullOrWhiteSpace(latestNumber) || !latestNumber.StartsWith(prefix))
        {
            return $"{prefix}000001";
        }

        var numericPart = latestNumber[prefix.Length..];

        if (!int.TryParse(numericPart, out var currentNumber))
        {
            return $"{prefix}000001";
        }

        return $"{prefix}{(currentNumber + 1):D6}";
    }
}