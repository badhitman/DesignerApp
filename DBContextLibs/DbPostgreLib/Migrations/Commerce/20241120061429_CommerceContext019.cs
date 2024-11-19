using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext019 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RubricId",
                table: "WarehouseDocuments",
                newName: "WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseDocuments_RubricId",
                table: "WarehouseDocuments",
                newName: "IX_WarehouseDocuments_WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "WarehouseDocuments",
                newName: "RubricId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseDocuments_WarehouseId",
                table: "WarehouseDocuments",
                newName: "IX_WarehouseDocuments_RubricId");
        }
    }
}
