using InstantProforms.Domain.Common;

namespace InstantProforms.Domain.Entities;

/// <summary>
/// Represents a line item inside a proforms.
/// </summary>
public sealed class ProformItem : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the proforms identifier.
    /// </summary>
    public Guid ProformId { get; set; }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the line total.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the related proforms.
    /// </summary>
    public Proform proforms { get; set; } = null!;
}