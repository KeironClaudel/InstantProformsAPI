using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstantProforms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClientsAndExpandedProforms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Proforms",
                newName: "Location");

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "Proforms",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientIdentificationNumber",
                table: "Proforms",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientIdentificationType",
                table: "Proforms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "Proforms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "Proforms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentConditions",
                table: "Proforms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScopeOfWork",
                table: "Proforms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceConditions",
                table: "Proforms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceDescription",
                table: "Proforms",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IdentificationType = table.Column<int>(type: "integer", nullable: true),
                    IdentificationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proforms_ClientId",
                table: "Proforms",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId_IdentificationType_IdentificationNumber",
                table: "Clients",
                columns: new[] { "CompanyId", "IdentificationType", "IdentificationNumber" },
                unique: true,
                filter: "\"IdentificationNumber\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId_Name",
                table: "Clients",
                columns: new[] { "CompanyId", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_Proforms_Clients_ClientId",
                table: "Proforms",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proforms_Clients_ClientId",
                table: "Proforms");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Proforms_ClientId",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "ClientIdentificationNumber",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "ClientIdentificationType",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "PaymentConditions",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "ScopeOfWork",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "ServiceConditions",
                table: "Proforms");

            migrationBuilder.DropColumn(
                name: "ServiceDescription",
                table: "Proforms");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Proforms",
                newName: "Notes");
        }
    }
}
