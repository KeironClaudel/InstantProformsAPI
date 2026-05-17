using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstantProforms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _20260516000100_ProtectClientIdentificationNumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientIdentificationNumberEncrypted",
                table: "Proforms",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumberEncrypted",
                table: "Clients",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumberHash",
                table: "Clients",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId_IdentificationType_IdentificationNumberHa~",
                table: "Clients",
                columns: new[] { "CompanyId", "IdentificationType", "IdentificationNumberHash" },
                unique: true,
                filter: "\"IdentificationNumberHash\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_CompanyId_IdentificationType_IdentificationNumberHa~",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ClientIdentificationNumberEncrypted",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "IdentificationNumberEncrypted",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IdentificationNumberHash",
                table: "Clients");
        }
    }
}
