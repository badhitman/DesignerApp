using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext023 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OffersAvailability_Goods_GoodsId",
                table: "OffersAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfOrdersDocuments_Goods_GoodsId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Goods_GoodsId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropIndex(
                name: "IX_OffersAvailability_WarehouseId",
                table: "OffersAvailability");

            migrationBuilder.RenameColumn(
                name: "GoodsId",
                table: "RowsOfWarehouseDocuments",
                newName: "NomenclatureId");

            migrationBuilder.RenameIndex(
                name: "IX_RowsOfWarehouseDocuments_GoodsId",
                table: "RowsOfWarehouseDocuments",
                newName: "IX_RowsOfWarehouseDocuments_NomenclatureId");

            migrationBuilder.RenameColumn(
                name: "GoodsId",
                table: "RowsOfOrdersDocuments",
                newName: "NomenclatureId");

            migrationBuilder.RenameIndex(
                name: "IX_RowsOfOrdersDocuments_GoodsId",
                table: "RowsOfOrdersDocuments",
                newName: "IX_RowsOfOrdersDocuments_NomenclatureId");

            migrationBuilder.RenameColumn(
                name: "GoodsId",
                table: "OffersAvailability",
                newName: "NomenclatureId");

            migrationBuilder.RenameIndex(
                name: "IX_OffersAvailability_GoodsId",
                table: "OffersAvailability",
                newName: "IX_OffersAvailability_NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersAvailability_WarehouseId_OfferId",
                table: "OffersAvailability",
                columns: new[] { "WarehouseId", "OfferId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OffersAvailability_Goods_NomenclatureId",
                table: "OffersAvailability",
                column: "NomenclatureId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfOrdersDocuments_Goods_NomenclatureId",
                table: "RowsOfOrdersDocuments",
                column: "NomenclatureId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Goods_NomenclatureId",
                table: "RowsOfWarehouseDocuments",
                column: "NomenclatureId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OffersAvailability_Goods_NomenclatureId",
                table: "OffersAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfOrdersDocuments_Goods_NomenclatureId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Goods_NomenclatureId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropIndex(
                name: "IX_OffersAvailability_WarehouseId_OfferId",
                table: "OffersAvailability");

            migrationBuilder.RenameColumn(
                name: "NomenclatureId",
                table: "RowsOfWarehouseDocuments",
                newName: "GoodsId");

            migrationBuilder.RenameIndex(
                name: "IX_RowsOfWarehouseDocuments_NomenclatureId",
                table: "RowsOfWarehouseDocuments",
                newName: "IX_RowsOfWarehouseDocuments_GoodsId");

            migrationBuilder.RenameColumn(
                name: "NomenclatureId",
                table: "RowsOfOrdersDocuments",
                newName: "GoodsId");

            migrationBuilder.RenameIndex(
                name: "IX_RowsOfOrdersDocuments_NomenclatureId",
                table: "RowsOfOrdersDocuments",
                newName: "IX_RowsOfOrdersDocuments_GoodsId");

            migrationBuilder.RenameColumn(
                name: "NomenclatureId",
                table: "OffersAvailability",
                newName: "GoodsId");

            migrationBuilder.RenameIndex(
                name: "IX_OffersAvailability_NomenclatureId",
                table: "OffersAvailability",
                newName: "IX_OffersAvailability_GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersAvailability_WarehouseId",
                table: "OffersAvailability",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_OffersAvailability_Goods_GoodsId",
                table: "OffersAvailability",
                column: "GoodsId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfOrdersDocuments_Goods_GoodsId",
                table: "RowsOfOrdersDocuments",
                column: "GoodsId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Goods_GoodsId",
                table: "RowsOfWarehouseDocuments",
                column: "GoodsId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
