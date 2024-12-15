using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext029 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkScheduleBaseModelDB_Nomenclatures_NomenclatureId",
                table: "WorkScheduleBaseModelDB");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkScheduleBaseModelDB_Offers_OfferId",
                table: "WorkScheduleBaseModelDB");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkScheduleBaseModelDB",
                table: "WorkScheduleBaseModelDB");

            migrationBuilder.DropIndex(
                name: "IX_WorkScheduleBaseModelDB_Weekday",
                table: "WorkScheduleBaseModelDB");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "WorkScheduleBaseModelDB");

            migrationBuilder.DropColumn(
                name: "Weekday",
                table: "WorkScheduleBaseModelDB");

            migrationBuilder.RenameTable(
                name: "WorkScheduleBaseModelDB",
                newName: "WorksSchedulesCalendar");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_StartPart_EndPart",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_StartPart_EndPart");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_SortIndex_ParentId_ContextName",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_SortIndex_ParentId_ContextName");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_OfferId",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_OfferId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_NormalizedNameUpper",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_NormalizedNameUpper");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_NomenclatureId",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_NomenclatureId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_Name",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_Name");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_IsDisabled",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_IsDisabled");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_ExecutorIdentityUserId",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_ExecutorIdentityUserId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_DateScheduleCalendar",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_DateScheduleCalendar");

            migrationBuilder.RenameIndex(
                name: "IX_WorkScheduleBaseModelDB_ContextName",
                table: "WorksSchedulesCalendar",
                newName: "IX_WorksSchedulesCalendar_ContextName");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateScheduleCalendar",
                table: "WorksSchedulesCalendar",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorksSchedulesCalendar",
                table: "WorksSchedulesCalendar",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "WorksSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Weekday = table.Column<int>(type: "integer", nullable: false),
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
                    StartPart = table.Column<long>(type: "bigint", nullable: false),
                    EndPart = table.Column<long>(type: "bigint", nullable: false)
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
                name: "IX_WorksSchedules_ExecutorIdentityUserId",
                table: "WorksSchedules",
                column: "ExecutorIdentityUserId");

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
                name: "IX_WorksSchedules_StartPart_EndPart",
                table: "WorksSchedules",
                columns: new[] { "StartPart", "EndPart" });

            migrationBuilder.CreateIndex(
                name: "IX_WorksSchedules_Weekday",
                table: "WorksSchedules",
                column: "Weekday");

            migrationBuilder.AddForeignKey(
                name: "FK_WorksSchedulesCalendar_Nomenclatures_NomenclatureId",
                table: "WorksSchedulesCalendar",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorksSchedulesCalendar_Offers_OfferId",
                table: "WorksSchedulesCalendar",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorksSchedulesCalendar_Nomenclatures_NomenclatureId",
                table: "WorksSchedulesCalendar");

            migrationBuilder.DropForeignKey(
                name: "FK_WorksSchedulesCalendar_Offers_OfferId",
                table: "WorksSchedulesCalendar");

            migrationBuilder.DropTable(
                name: "WorksSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorksSchedulesCalendar",
                table: "WorksSchedulesCalendar");

            migrationBuilder.RenameTable(
                name: "WorksSchedulesCalendar",
                newName: "WorkScheduleBaseModelDB");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_StartPart_EndPart",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_StartPart_EndPart");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_SortIndex_ParentId_ContextName",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_SortIndex_ParentId_ContextName");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_OfferId",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_OfferId");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_NormalizedNameUpper",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_NormalizedNameUpper");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_NomenclatureId",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_NomenclatureId");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_Name",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_Name");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_IsDisabled",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_IsDisabled");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_ExecutorIdentityUserId",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_ExecutorIdentityUserId");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_DateScheduleCalendar",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_DateScheduleCalendar");

            migrationBuilder.RenameIndex(
                name: "IX_WorksSchedulesCalendar_ContextName",
                table: "WorkScheduleBaseModelDB",
                newName: "IX_WorkScheduleBaseModelDB_ContextName");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateScheduleCalendar",
                table: "WorkScheduleBaseModelDB",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "WorkScheduleBaseModelDB",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Weekday",
                table: "WorkScheduleBaseModelDB",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkScheduleBaseModelDB",
                table: "WorkScheduleBaseModelDB",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkScheduleBaseModelDB_Weekday",
                table: "WorkScheduleBaseModelDB",
                column: "Weekday");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkScheduleBaseModelDB_Nomenclatures_NomenclatureId",
                table: "WorkScheduleBaseModelDB",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkScheduleBaseModelDB_Offers_OfferId",
                table: "WorkScheduleBaseModelDB",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id");
        }
    }
}
