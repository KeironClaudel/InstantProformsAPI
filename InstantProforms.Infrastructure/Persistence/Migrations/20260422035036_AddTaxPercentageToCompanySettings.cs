using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstantProforms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxPercentageToCompanySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "CompanySettings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "CompanySettings");
        }
    }
}
