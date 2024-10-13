using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "Articles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedNameUpper",
                table: "Articles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SortIndex",
                table: "Articles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "NormalizedNameUpper",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SortIndex",
                table: "Articles");
        }
    }
}
