using InstantProforms.Application.Features.Proforms.Common;

namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines PDF generation operations for proforms.
/// </summary>
public interface IProformPdfService
{
    /// <summary>
    /// Generates a PDF document for the specified proform model.
    /// </summary>
    /// <param name="model">The proform PDF model.</param>
    /// <returns>The generated PDF as a byte array.</returns>
    byte[] Generate(ProformPdfModel model);
}