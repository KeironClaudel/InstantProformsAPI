namespace InstantProforms.Application.Common.Models;

/// <summary>
/// Represents Supabase Storage configuration for tenant assets.
/// </summary>
public sealed class SupabaseStorageSettings
{
    /// <summary>
    /// Gets or sets the Supabase project base URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service role key used by the backend.
    /// </summary>
    public string ServiceRoleKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the storage bucket used for company assets.
    /// </summary>
    public string BucketName { get; set; } = "company-assets";

    /// <summary>
    /// Gets or sets the folder prefix used for company logos.
    /// </summary>
    public string CompanyLogosFolder { get; set; } = "uploads/company-logos";
}
