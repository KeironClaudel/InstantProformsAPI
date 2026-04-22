using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstantProforms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProformFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProformItems_Proforms_proformsId",
                table: "ProformItems");

            migrationBuilder.RenameColumn(
                name: "proformsId",
                table: "ProformItems",
                newName: "ProformId");

            migrationBuilder.RenameIndex(
                name: "IX_ProformItems_proformsId",
                table: "ProformItems",
                newName: "IX_ProformItems_ProformId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProformItems_Proforms_ProformId",
                table: "ProformItems",
                column: "ProformId",
                principalTable: "Proforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProformItems_Proforms_ProformId",
                table: "ProformItems");

            migrationBuilder.RenameColumn(
                name: "ProformId",
                table: "ProformItems",
                newName: "proformsId");

            migrationBuilder.RenameIndex(
                name: "IX_ProformItems_ProformId",
                table: "ProformItems",
                newName: "IX_ProformItems_proformsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProformItems_Proforms_proformsId",
                table: "ProformItems",
                column: "proformsId",
                principalTable: "Proforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
