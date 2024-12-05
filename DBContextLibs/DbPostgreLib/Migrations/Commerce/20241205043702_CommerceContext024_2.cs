using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext024_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Nomenclatures_GoodsId",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "GoodsId",
                table: "Offers",
                newName: "NomenclatureId");

            migrationBuilder.RenameIndex(
                name: "IX_Offers_GoodsId",
                table: "Offers",
                newName: "IX_Offers_NomenclatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Nomenclatures_NomenclatureId",
                table: "Offers",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Nomenclatures_NomenclatureId",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "NomenclatureId",
                table: "Offers",
                newName: "GoodsId");

            migrationBuilder.RenameIndex(
                name: "IX_Offers_NomenclatureId",
                table: "Offers",
                newName: "IX_Offers_GoodsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Nomenclatures_GoodsId",
                table: "Offers",
                column: "GoodsId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
