using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext026_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExecutorIdentityUserId",
                table: "WorksSchedules",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorksSchedulesCalendar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateScheduleCalendar = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SortIndex = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    ContextName = table.Column<string>(type: "text", nullable: true),
                    NormalizedNameUpper = table.Column<string>(type: "text", nullable: true),
                    ExecutorIdentityUserId = table.Column<string>(type: "text", nullable: true),
                    NomenclatureId = table.Column<int>(type: "integer", nullable: true),
                    OfferId = table.Column<int>(type: "integer", nullable: true),
                    StartPart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndPart = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorksSchedulesCalendar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorksSchedulesCalendar_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorksSchedulesCalendar_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_ExecutorIdentityUserId",
                table: "WorksSchedules",
                column: "ExecutorIdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedulesCalendar_ContextName",
                table: "WorksSchedulesCalendar",
                column: "ContextName");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedulesCalendar_DateScheduleCalendar",
                table: "WorksSchedulesCalendar",
                column: "DateScheduleCalendar");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedulesCalendar_ExecutorIdentityUserId",
                table: "WorksSchedulesCalendar",
                column: "ExecutorIdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedulesCalendar_IsDisabled",
                table: "WorksSchedulesCalendar",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedulesCalendar_Name",
                table: "WorksSchedulesCalendar",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedulesCalendar_NomenclatureId",
                table: "WorksSchedulesCalendar",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedulesCalendar_NormalizedNameUpper",
                table: "WorksSchedulesCalendar",
                column: "NormalizedNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedulesCalendar_OfferId",
                table: "WorksSchedulesCalendar",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedulesCalendar_SortIndex_ParentId_ContextName",
                table: "WorksSchedulesCalendar",
                columns: new[] { "SortIndex", "ParentId", "ContextName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorksSchedulesCalendar");

            migrationBuilder.DropIndex(
                name: "IX_WorksSchedules_ExecutorIdentityUserId",
                table: "WorksSchedules");

            migrationBuilder.DropColumn(
                name: "ExecutorIdentityUserId",
                table: "WorksSchedules");
        }
    }
}
