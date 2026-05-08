using InstantProforms.Domain.Common;
using InstantProforms.Domain.Enums;

namespace InstantProforms.Domain.Entities;

/// <summary>
/// Represents a reusable client record owned by a company.
/// </summary>
public sealed class Client : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the owning company identifier.
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the client display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the client phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the client identification type.
    /// </summary>
    public ClientIdentificationType? IdentificationType { get; set; }

    /// <summary>
    /// Gets or sets the client identification number.
    /// </summary>
    public string? IdentificationNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the client is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the related company.
    /// </summary>
    public Company Company { get; set; } = null!;

    /// <summary>
    /// Gets or sets the proforms created from this client.
    /// </summary>
    public ICollection<Proform> Proforms { get; set; } = new List<Proform>();
}
