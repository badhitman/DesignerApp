using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext009 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_UpdatedAtUTC",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUTC",
                table: "Articles");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAtUTC",
                table: "Articles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_LastUpdatedAtUTC",
                table: "Articles",
                column: "LastUpdatedAtUTC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_LastUpdatedAtUTC",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAtUTC",
                table: "Articles");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUTC",
                table: "Articles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UpdatedAtUTC",
                table: "Articles",
                column: "UpdatedAtUTC");
        }
    }
}
