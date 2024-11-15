using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext015_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RowsOfWarehouseDocuments_RubricId",
                table: "RowsOfWarehouseDocuments",
                column: "RubricId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_RubricId",
                table: "RowsOfOrdersDocuments",
                column: "RubricId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersAvailability_RubricId",
                table: "OffersAvailability",
                column: "RubricId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RowsOfWarehouseDocuments_RubricId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RowsOfOrdersDocuments_RubricId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropIndex(
                name: "IX_OffersAvailability_RubricId",
                table: "OffersAvailability");
        }
    }
}
