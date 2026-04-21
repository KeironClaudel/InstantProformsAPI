using InstantProforms.Application.Features.Proforms.GetProformById;

namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines PDF generation operations for proforms.
/// </summary>
public interface IProformPdfService
{
    /// <summary>
    /// Generates a PDF document for the specified proform.
    /// </summary>
    /// <param name="proform">The proform data.</param>
    /// <returns>The generated PDF as a byte array.</returns>
    byte[] Generate(GetProformByIdResponse proform);
}