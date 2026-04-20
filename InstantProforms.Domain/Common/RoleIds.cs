namespace InstantProforms.Domain.Common;

/// <summary>
/// Provides predefined role identifiers.
/// </summary>
public static class RoleIds
{
    /// <summary>
    /// Gets the owner role identifier.
    /// </summary>
    public static readonly Guid Owner = Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <summary>
    /// Gets the admin role identifier.
    /// </summary>
    public static readonly Guid Admin = Guid.Parse("22222222-2222-2222-2222-222222222222");

    /// <summary>
    /// Gets the employee role identifier.
    /// </summary>
    public static readonly Guid Employee = Guid.Parse("33333333-3333-3333-3333-333333333333");
}