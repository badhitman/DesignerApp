using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Issues",
                newName: "CreatedAtUTC");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_CreatedAt",
                table: "Issues",
                newName: "IX_Issues_CreatedAtUTC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAtUTC",
                table: "Issues",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_CreatedAtUTC",
                table: "Issues",
                newName: "IX_Issues_CreatedAt");
        }
    }
}
