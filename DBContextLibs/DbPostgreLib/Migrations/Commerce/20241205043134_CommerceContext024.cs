using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OffersAvailability_Goods_NomenclatureId",
                table: "OffersAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_OffersAvailability_OffersGoods_OfferId",
                table: "OffersAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_OffersGoods_Goods_GoodsId",
                table: "OffersGoods");

            migrationBuilder.DropForeignKey(
                name: "FK_PricesRules_OffersGoods_OfferId",
                table: "PricesRules");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfOrdersDocuments_Goods_NomenclatureId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfOrdersDocuments_OffersGoods_OfferId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Goods_NomenclatureId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfWarehouseDocuments_OffersGoods_OfferId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OffersGoods",
                table: "OffersGoods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Goods",
                table: "Goods");

            migrationBuilder.RenameTable(
                name: "OffersGoods",
                newName: "Offers");

            migrationBuilder.RenameTable(
                name: "Goods",
                newName: "Nomenclatures");

            migrationBuilder.RenameIndex(
                name: "IX_OffersGoods_Name",
                table: "Offers",
                newName: "IX_Offers_Name");

            migrationBuilder.RenameIndex(
                name: "IX_OffersGoods_IsDisabled",
                table: "Offers",
                newName: "IX_Offers_IsDisabled");

            migrationBuilder.RenameIndex(
                name: "IX_OffersGoods_GoodsId",
                table: "Offers",
                newName: "IX_Offers_GoodsId");

            migrationBuilder.RenameIndex(
                name: "IX_Goods_SortIndex_ParentId",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_SortIndex_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Goods_NormalizedNameUpper",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_NormalizedNameUpper");

            migrationBuilder.RenameIndex(
                name: "IX_Goods_Name",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Goods_IsDisabled",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_IsDisabled");

            migrationBuilder.RenameIndex(
                name: "IX_Goods_ContextName",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_ContextName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Offers",
                table: "Offers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Nomenclatures",
                table: "Nomenclatures",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Nomenclatures_GoodsId",
                table: "Offers",
                column: "GoodsId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OffersAvailability_Nomenclatures_NomenclatureId",
                table: "OffersAvailability",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OffersAvailability_Offers_OfferId",
                table: "OffersAvailability",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PricesRules_Offers_OfferId",
                table: "PricesRules",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfOrdersDocuments_Nomenclatures_NomenclatureId",
                table: "RowsOfOrdersDocuments",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfOrdersDocuments_Offers_OfferId",
                table: "RowsOfOrdersDocuments",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Nomenclatures_NomenclatureId",
                table: "RowsOfWarehouseDocuments",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Offers_OfferId",
                table: "RowsOfWarehouseDocuments",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Nomenclatures_GoodsId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_OffersAvailability_Nomenclatures_NomenclatureId",
                table: "OffersAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_OffersAvailability_Offers_OfferId",
                table: "OffersAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_PricesRules_Offers_OfferId",
                table: "PricesRules");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfOrdersDocuments_Nomenclatures_NomenclatureId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfOrdersDocuments_Offers_OfferId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Nomenclatures_NomenclatureId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Offers_OfferId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Offers",
                table: "Offers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Nomenclatures",
                table: "Nomenclatures");

            migrationBuilder.RenameTable(
                name: "Offers",
                newName: "OffersGoods");

            migrationBuilder.RenameTable(
                name: "Nomenclatures",
                newName: "Goods");

            migrationBuilder.RenameIndex(
                name: "IX_Offers_Name",
                table: "OffersGoods",
                newName: "IX_OffersGoods_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Offers_IsDisabled",
                table: "OffersGoods",
                newName: "IX_OffersGoods_IsDisabled");

            migrationBuilder.RenameIndex(
                name: "IX_Offers_GoodsId",
                table: "OffersGoods",
                newName: "IX_OffersGoods_GoodsId");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_SortIndex_ParentId",
                table: "Goods",
                newName: "IX_Goods_SortIndex_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_NormalizedNameUpper",
                table: "Goods",
                newName: "IX_Goods_NormalizedNameUpper");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_Name",
                table: "Goods",
                newName: "IX_Goods_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_IsDisabled",
                table: "Goods",
                newName: "IX_Goods_IsDisabled");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_ContextName",
                table: "Goods",
                newName: "IX_Goods_ContextName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OffersGoods",
                table: "OffersGoods",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Goods",
                table: "Goods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OffersAvailability_Goods_NomenclatureId",
                table: "OffersAvailability",
                column: "NomenclatureId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OffersAvailability_OffersGoods_OfferId",
                table: "OffersAvailability",
                column: "OfferId",
                principalTable: "OffersGoods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OffersGoods_Goods_GoodsId",
                table: "OffersGoods",
                column: "GoodsId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PricesRules_OffersGoods_OfferId",
                table: "PricesRules",
                column: "OfferId",
                principalTable: "OffersGoods",
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
                name: "FK_RowsOfOrdersDocuments_OffersGoods_OfferId",
                table: "RowsOfOrdersDocuments",
                column: "OfferId",
                principalTable: "OffersGoods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfWarehouseDocuments_Goods_NomenclatureId",
                table: "RowsOfWarehouseDocuments",
                column: "NomenclatureId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RowsOfWarehouseDocuments_OffersGoods_OfferId",
                table: "RowsOfWarehouseDocuments",
                column: "OfferId",
                principalTable: "OffersGoods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
