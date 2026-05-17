using InstantProforms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstantProforms.Infrastructure.Persistence.Migrations;

/// <summary>
/// Adds explicit deny-all policies for Supabase anon/authenticated roles on public tables.
/// The application uses the backend as the only data access path, so direct client access should remain blocked.
/// </summary>
[DbContext(typeof(AppDbContext))]
[Migration("20260517073000_AddSupabaseDenyPolicies")]
public sealed class AddSupabaseDenyPolicies : Migration
{
    private static readonly string[] PublicTables =
    [
        "__EFMigrationsHistory",
        "Companies",
        "Users",
        "Roles",
        "RefreshTokens",
        "Proforms",
        "ProformItems",
        "PasswordResetTokens",
        "ProformShareTokens",
        "CompanySettings",
        "StoredFiles",
        "Clients"
    ];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var tableName in PublicTables)
        {
            migrationBuilder.Sql($@"
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_policies
        WHERE schemaname = 'public'
          AND tablename = '{tableName}'
          AND policyname = 'deny_all_anon_authenticated')
    THEN
        EXECUTE 'CREATE POLICY ""deny_all_anon_authenticated"" ON public.""{tableName}""
            AS PERMISSIVE
            FOR ALL
            TO anon, authenticated
            USING (false)
            WITH CHECK (false)';
    END IF;
END
$$;");
        }
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var tableName in PublicTables)
        {
            migrationBuilder.Sql($@"DROP POLICY IF EXISTS ""deny_all_anon_authenticated"" ON public.""{tableName}"";");
        }
    }
}
