using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext030_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DateScheduleCalendar",
                table: "WorkScheduleBaseModelDB",
                type: "character varying(10)",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateScheduleCalendar",
                table: "WorkScheduleBaseModelDB",
                type: "date",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldNullable: true);
        }
    }
}
