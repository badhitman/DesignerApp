using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext020_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RowsOfWarehouseDocuments_WarehouseDocumentId",
                table: "RowsOfWarehouseDocuments",
                column: "WarehouseDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_AddressForOrderTabId",
                table: "RowsOfOrdersDocuments",
                column: "AddressForOrderTabId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RowsOfWarehouseDocuments_WarehouseDocumentId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RowsOfOrdersDocuments_AddressForOrderTabId",
                table: "RowsOfOrdersDocuments");
        }
    }
}
