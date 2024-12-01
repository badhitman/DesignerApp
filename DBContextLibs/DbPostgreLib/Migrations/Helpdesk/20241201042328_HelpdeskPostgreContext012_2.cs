using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext012_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StepIssue",
                table: "Issues",
                newName: "StatusDocument");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_StepIssue",
                table: "Issues",
                newName: "IX_Issues_StatusDocument");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusDocument",
                table: "Issues",
                newName: "StepIssue");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_StatusDocument",
                table: "Issues",
                newName: "IX_Issues_StepIssue");
        }
    }
}
