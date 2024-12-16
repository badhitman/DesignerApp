using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext031 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdersAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StatusDocument = table.Column<int>(type: "integer", nullable: false),
                    AuthorIdentityUserId = table.Column<string>(type: "text", nullable: false),
                    ExternalDocumentId = table.Column<string>(type: "text", nullable: true),
                    Information = table.Column<string>(type: "text", nullable: true),
                    HelpdeskId = table.Column<int>(type: "integer", nullable: true),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdersAttendances_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RowsOfOrdersAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateAttendance = table.Column<DateOnly>(type: "date", nullable: false),
                    OrderDocumentId = table.Column<int>(type: "integer", nullable: true),
                    StartPart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndPart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Version = table.Column<Guid>(type: "uuid", nullable: false),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_OrdersAttendances_Name",
                table: "OrdersAttendances",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersAttendances_OrganizationId",
                table: "OrdersAttendances",
                column: "OrganizationId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RowsOfOrdersAttendances");

            migrationBuilder.DropTable(
                name: "OrdersAttendances");
        }
    }
}
