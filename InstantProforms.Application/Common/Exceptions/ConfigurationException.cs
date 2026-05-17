namespace InstantProforms.Application.Common.Exceptions;

/// <summary>
/// Represents a safe configuration error that can be shown to the client.
/// </summary>
public sealed class ConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The safe error message.</param>
    public ConfigurationException(string message)
        : base(message)
    {
    }
}
