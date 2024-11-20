using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext019_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryData",
                table: "WarehouseDocuments",
                newName: "DeliveryDate");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseDocuments_DeliveryData",
                table: "WarehouseDocuments",
                newName: "IX_WarehouseDocuments_DeliveryDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "WarehouseDocuments",
                newName: "DeliveryData");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseDocuments_DeliveryDate",
                table: "WarehouseDocuments",
                newName: "IX_WarehouseDocuments_DeliveryData");
        }
    }
}
