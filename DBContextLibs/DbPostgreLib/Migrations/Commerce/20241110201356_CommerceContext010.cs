using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OffersAvailability",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    GoodsId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffersAvailability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OffersAvailability_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OffersAvailability_OffersGoods_OfferId",
                        column: x => x.OfferId,
                        principalTable: "OffersGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeliveryData = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExternalDocumentId = table.Column<string>(type: "text", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RowsOfWarehouseDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseDocumentId = table.Column<int>(type: "integer", nullable: false),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    GoodsId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowsOfWarehouseDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RowsOfWarehouseDocuments_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsOfWarehouseDocuments_OffersGoods_OfferId",
                        column: x => x.OfferId,
                        principalTable: "OffersGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsOfWarehouseDocuments_WarehouseDocuments_WarehouseDocume~",
                        column: x => x.WarehouseDocumentId,
                        principalTable: "WarehouseDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OffersAvailability_GoodsId",
                table: "OffersAvailability",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersAvailability_OfferId",
                table: "OffersAvailability",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfWarehouseDocuments_GoodsId",
                table: "RowsOfWarehouseDocuments",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfWarehouseDocuments_OfferId",
                table: "RowsOfWarehouseDocuments",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfWarehouseDocuments_WarehouseDocumentId",
                table: "RowsOfWarehouseDocuments",
                column: "WarehouseDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseDocuments_DeliveryData",
                table: "WarehouseDocuments",
                column: "DeliveryData");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseDocuments_IsDisabled",
                table: "WarehouseDocuments",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseDocuments_Name",
                table: "WarehouseDocuments",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OffersAvailability");

            migrationBuilder.DropTable(
                name: "RowsOfWarehouseDocuments");

            migrationBuilder.DropTable(
                name: "WarehouseDocuments");
        }
    }
}
