using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext034_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RowsOfOrdersAttendances");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateAttendance",
                table: "OrdersAttendances",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndPart",
                table: "OrdersAttendances",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "NomenclatureId",
                table: "OrdersAttendances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OfferId",
                table: "OrdersAttendances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartPart",
                table: "OrdersAttendances",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_OrdersAttendances_NomenclatureId",
                table: "OrdersAttendances",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersAttendances_OfferId",
                table: "OrdersAttendances",
                column: "OfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersAttendances_Nomenclatures_NomenclatureId",
                table: "OrdersAttendances",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersAttendances_Offers_OfferId",
                table: "OrdersAttendances",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersAttendances_Nomenclatures_NomenclatureId",
                table: "OrdersAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdersAttendances_Offers_OfferId",
                table: "OrdersAttendances");

            migrationBuilder.DropIndex(
                name: "IX_OrdersAttendances_NomenclatureId",
                table: "OrdersAttendances");

            migrationBuilder.DropIndex(
                name: "IX_OrdersAttendances_OfferId",
                table: "OrdersAttendances");

            migrationBuilder.DropColumn(
                name: "DateAttendance",
                table: "OrdersAttendances");

            migrationBuilder.DropColumn(
                name: "EndPart",
                table: "OrdersAttendances");

            migrationBuilder.DropColumn(
                name: "NomenclatureId",
                table: "OrdersAttendances");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "OrdersAttendances");

            migrationBuilder.DropColumn(
                name: "StartPart",
                table: "OrdersAttendances");

            migrationBuilder.CreateTable(
                name: "RowsOfOrdersAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    OrderDocumentId = table.Column<int>(type: "integer", nullable: true),
                    DateAttendance = table.Column<DateOnly>(type: "date", nullable: false),
                    EndPart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    StartPart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowsOfOrdersAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RowsOfOrdersAttendances_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsOfOrdersAttendances_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsOfOrdersAttendances_OrdersAttendances_OrderDocumentId",
                        column: x => x.OrderDocumentId,
                        principalTable: "OrdersAttendances",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersAttendances_NomenclatureId",
                table: "RowsOfOrdersAttendances",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersAttendances_OfferId",
                table: "RowsOfOrdersAttendances",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersAttendances_OrderDocumentId",
                table: "RowsOfOrdersAttendances",
                column: "OrderDocumentId");
        }
    }
}
