using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstantProforms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _20260516001000_RemoveLegacyPlaintextIdentificationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_CompanyId_IdentificationType_IdentificationNumber",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ClientIdentificationNumber",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "IdentificationNumber",
                table: "Clients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientIdentificationNumber",
                table: "Proforms",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumber",
                table: "Clients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId_IdentificationType_IdentificationNumber",
                table: "Clients",
                columns: new[] { "CompanyId", "IdentificationType", "IdentificationNumber" },
                unique: true,
                filter: "\"IdentificationNumber\" IS NOT NULL");
        }
    }
}
