using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstantProforms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPerCompanyResendSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResendApiKeyEncrypted",
                table: "CompanySettings",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResendReplyToEmailEncrypted",
                table: "CompanySettings",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResendSenderEmailEncrypted",
                table: "CompanySettings",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResendSenderNameEncrypted",
                table: "CompanySettings",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResendApiKeyEncrypted",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "ResendReplyToEmailEncrypted",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "ResendSenderEmailEncrypted",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "ResendSenderNameEncrypted",
                table: "CompanySettings");
        }
    }
}
