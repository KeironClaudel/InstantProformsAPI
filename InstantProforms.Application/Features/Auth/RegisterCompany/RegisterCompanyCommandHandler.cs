using MediatR;
using Microsoft.EntityFrameworkCore;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Domain.Common;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Features.Auth.RegisterCompany;

public sealed class RegisterCompanyCommandHandler
    : IRequestHandler<RegisterCompanyCommand, RegisterCompanyResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCompanyCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterCompanyResponse> Handle(
        RegisterCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var slugExists = await _context.Companies
            .AnyAsync(x => x.Slug == request.CompanySlug, cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException("The company slug is already in use.");
        }

        var ownerEmailExists = await _context.Users
            .AnyAsync(x => x.Email == request.OwnerEmail, cancellationToken);

        if (ownerEmailExists)
        {
            throw new InvalidOperationException("The owner email is already in use.");
        }

        var ownerRole = await _context.Roles
            .FirstOrDefaultAsync(x => x.Id == RoleIds.Owner && x.IsActive, cancellationToken);

        if (ownerRole is null)
        {
            throw new InvalidOperationException("The Owner role was not found.");
        }

        var utcNow = DateTime.UtcNow;

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.CompanyName,
            Slug = request.CompanySlug,
            Email = request.CompanyEmail,
            Phone = request.CompanyPhone,
            Address = request.CompanyAddress,
            IsActive = true,
            CreatedAtUtc = utcNow
        };

        var ownerUser = new User
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            RoleId = ownerRole.Id,
            FullName = request.OwnerFullName,
            Email = request.OwnerEmail,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            CreatedAtUtc = utcNow
        };

        _context.Companies.Add(company);
        _context.Users.Add(ownerUser);

        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterCompanyResponse(
            company.Id,
            ownerUser.Id,
            "Company and owner user registered successfully.");
    }
}