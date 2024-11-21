using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext021 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RowsOfWarehouseDocuments_Quantity",
                table: "RowsOfWarehouseDocuments",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_Quantity",
                table: "RowsOfOrdersDocuments",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_OffersAvailability_Quantity",
                table: "OffersAvailability",
                column: "Quantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RowsOfWarehouseDocuments_Quantity",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RowsOfOrdersDocuments_Quantity",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropIndex(
                name: "IX_OffersAvailability_Quantity",
                table: "OffersAvailability");
        }
    }
}
