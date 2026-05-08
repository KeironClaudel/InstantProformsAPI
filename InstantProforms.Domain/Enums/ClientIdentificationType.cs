namespace InstantProforms.Domain.Enums;

/// <summary>
/// Defines the supported client identification document types.
/// </summary>
public enum ClientIdentificationType
{
    /// <summary>
    /// A natural person's identification card.
    /// </summary>
    PhysicalId = 1,

    /// <summary>
    /// A legal entity identification number.
    /// </summary>
    LegalEntityId = 2
}
