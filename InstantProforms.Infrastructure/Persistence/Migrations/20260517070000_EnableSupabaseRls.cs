using InstantProforms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstantProforms.Infrastructure.Persistence.Migrations;

/// <summary>
/// Enables Supabase Row Level Security on public tables so anon/authenticated roles cannot read or mutate
/// application data directly. The backend connects as postgres and keeps the current application behavior.
/// </summary>
[DbContext(typeof(AppDbContext))]
[Migration("20260517070000_EnableSupabaseRls")]
public sealed class EnableSupabaseRls : Migration
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
            migrationBuilder.Sql($@"ALTER TABLE public.""{tableName}"" ENABLE ROW LEVEL SECURITY;");
        }
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var tableName in PublicTables)
        {
            migrationBuilder.Sql($@"ALTER TABLE public.""{tableName}"" DISABLE ROW LEVEL SECURITY;");
        }
    }
}
