using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext020 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RowsOfWarehouseDocuments_WarehouseDocumentId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RowsOfOrdersDocuments_AddressForOrderTabId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfWarehouseDocuments_WarehouseDocumentId_OfferId",
                table: "RowsOfWarehouseDocuments",
                columns: new[] { "WarehouseDocumentId", "OfferId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_AddressForOrderTabId_OfferId",
                table: "RowsOfOrdersDocuments",
                columns: new[] { "AddressForOrderTabId", "OfferId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RowsOfWarehouseDocuments_WarehouseDocumentId_OfferId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RowsOfOrdersDocuments_AddressForOrderTabId_OfferId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfWarehouseDocuments_WarehouseDocumentId",
                table: "RowsOfWarehouseDocuments",
                column: "WarehouseDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_AddressForOrderTabId",
                table: "RowsOfOrdersDocuments",
                column: "AddressForOrderTabId");
        }
    }
}
