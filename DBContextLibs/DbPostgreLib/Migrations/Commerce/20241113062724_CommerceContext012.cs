using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RowsOfOffersAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfferAvailabilityId = table.Column<int>(type: "integer", nullable: false),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    GoodsId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    WarehouseDocumentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowsOfOffersAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RowsOfOffersAvailabilities_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsOfOffersAvailabilities_OffersAvailability_OfferAvailabi~",
                        column: x => x.OfferAvailabilityId,
                        principalTable: "OffersAvailability",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsOfOffersAvailabilities_OffersGoods_OfferId",
                        column: x => x.OfferId,
                        principalTable: "OffersGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsOfOffersAvailabilities_WarehouseDocuments_WarehouseDocu~",
                        column: x => x.WarehouseDocumentId,
                        principalTable: "WarehouseDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOffersAvailabilities_GoodsId",
                table: "RowsOfOffersAvailabilities",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOffersAvailabilities_OfferAvailabilityId",
                table: "RowsOfOffersAvailabilities",
                column: "OfferAvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOffersAvailabilities_OfferId",
                table: "RowsOfOffersAvailabilities",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOffersAvailabilities_WarehouseDocumentId",
                table: "RowsOfOffersAvailabilities",
                column: "WarehouseDocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RowsOfOffersAvailabilities");
        }
    }
}
