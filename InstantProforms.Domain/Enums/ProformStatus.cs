namespace InstantProforms.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a proforms.
/// </summary>
public enum ProformStatus
{
    /// <summary>
    /// The proforms is being prepared and has not been sent yet.
    /// </summary>
    Draft = 1,

    /// <summary>
    /// The proforms has been sent to the client.
    /// </summary>
    Sent = 2,

    /// <summary>
    /// The proforms has been accepted by the client.
    /// </summary>
    Accepted = 3,

    /// <summary>
    /// The proforms has been rejected by the client.
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// The proforms is no longer valid.
    /// </summary>
    Expired = 5,

    /// <summary>
    /// The proforms was cancelled.
    /// </summary>
    Cancelled = 6
}