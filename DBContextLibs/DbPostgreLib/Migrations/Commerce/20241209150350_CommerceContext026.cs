using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorksSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: true),
                    OfferId = table.Column<int>(type: "integer", nullable: true),
                    Weekday = table.Column<int>(type: "integer", nullable: false),
                    StartPart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndPart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SortIndex = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    ContextName = table.Column<string>(type: "text", nullable: true),
                    NormalizedNameUpper = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorksSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorksSchedules_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorksSchedules_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_ContextName",
                table: "WorksSchedules",
                column: "ContextName");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_IsDisabled",
                table: "WorksSchedules",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_Name",
                table: "WorksSchedules",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_NomenclatureId",
                table: "WorksSchedules",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_NormalizedNameUpper",
                table: "WorksSchedules",
                column: "NormalizedNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_OfferId",
                table: "WorksSchedules",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_SortIndex_ParentId_ContextName",
                table: "WorksSchedules",
                columns: new[] { "SortIndex", "ParentId", "ContextName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_Weekday",
                table: "WorksSchedules",
                column: "Weekday");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorksSchedules");
        }
    }
}
