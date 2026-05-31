using MediatR;
using Microsoft.Extensions.Options;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Features.Proforms.CreateProformShareLink;

/// <summary>
/// Handles creation of temporary public share links for proforms.
/// </summary>
public sealed class CreateProformShareLinkCommandHandler
    : IRequestHandler<CreateProformShareLinkCommand, CreateProformShareLinkResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITokenHashService _tokenHashService;
    private readonly ProformShareSettings _proformShareSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProformShareLinkCommandHandler"/> class.
    /// </summary>
    public CreateProformShareLinkCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IJwtTokenService jwtTokenService,
        ITokenHashService tokenHashService,
        IOptions<ProformShareSettings> proformShareSettings)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _jwtTokenService = jwtTokenService;
        _tokenHashService = tokenHashService;
        _proformShareSettings = proformShareSettings.Value;
    }

    /// <inheritdoc />
    public async Task<CreateProformShareLinkResponse> Handle(
        CreateProformShareLinkCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var proform = await _unitOfWork.Proforms
            .GetByIdAsync(request.ProformId, _currentUserService.CompanyId.Value, cancellationToken);

        if (proform is null)
        {
            throw new InvalidOperationException("Quotation was not found.");
        }

        var rawToken = _jwtTokenService.GenerateRefreshToken();
        var tokenHash = _tokenHashService.ComputeHash(rawToken);

        var expirationMinutes = request.ExpirationMinutes ?? _proformShareSettings.DefaultExpirationMinutes;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var shareToken = new ProformShareToken
        {
            Id = Guid.NewGuid(),
            ProformId = proform.Id,
            TokenHash = tokenHash,
            ExpiresAtUtc = expiresAtUtc,
            IsSingleUse = request.IsSingleUse,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.ProformShareTokens.AddAsync(shareToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var publicDownloadBaseUrl = _proformShareSettings.PublicDownloadUrl.TrimEnd('/');
        var url = $"{publicDownloadBaseUrl}/{rawToken}";

        return new CreateProformShareLinkResponse(
            url,
            expiresAtUtc,
            request.IsSingleUse);
    }
}
