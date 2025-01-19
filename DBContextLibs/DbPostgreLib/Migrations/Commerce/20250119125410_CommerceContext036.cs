using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext036 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdersAttendances");

            migrationBuilder.CreateTable(
                name: "RecordsAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false),
                    DateExecute = table.Column<DateOnly>(type: "date", nullable: false),
                    StartPart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndPart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ContextName = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_RecordsAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordsAttendances_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordsAttendances_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordsAttendances_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordsAttendances_Name",
                table: "RecordsAttendances",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RecordsAttendances_NomenclatureId",
                table: "RecordsAttendances",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordsAttendances_OfferId",
                table: "RecordsAttendances",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordsAttendances_OrganizationId",
                table: "RecordsAttendances",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordsAttendances");

            migrationBuilder.CreateTable(
                name: "OrdersAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: false),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    AuthorIdentityUserId = table.Column<string>(type: "text", nullable: false),
                    ContextName = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateExecute = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EndPart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ExternalDocumentId = table.Column<string>(type: "text", nullable: true),
                    HelpdeskId = table.Column<int>(type: "integer", nullable: true),
                    Information = table.Column<string>(type: "text", nullable: true),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartPart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    StatusDocument = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdersAttendances_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdersAttendances_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdersAttendances_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdersAttendances_Name",
                table: "OrdersAttendances",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersAttendances_NomenclatureId",
                table: "OrdersAttendances",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersAttendances_OfferId",
                table: "OrdersAttendances",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersAttendances_OrganizationId",
                table: "OrdersAttendances",
                column: "OrganizationId");
        }
    }
}
