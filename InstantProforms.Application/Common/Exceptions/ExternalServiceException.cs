namespace InstantProforms.Application.Common.Exceptions;

/// <summary>
/// Represents a failure while communicating with an external dependency.
/// </summary>
public sealed class ExternalServiceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalServiceException"/> class.
    /// </summary>
    /// <param name="message">The safe error message.</param>
    public ExternalServiceException(string message)
        : base(message)
    {
    }
}
