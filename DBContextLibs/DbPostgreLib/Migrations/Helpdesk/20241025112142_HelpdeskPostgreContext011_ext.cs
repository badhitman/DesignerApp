using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext011_ext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rubrics_ContextName",
                table: "Rubrics",
                column: "ContextName");

            migrationBuilder.CreateIndex(
                name: "IX_Rubrics_NormalizedNameUpper",
                table: "Rubrics",
                column: "NormalizedNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AuthorIdentityUserId",
                table: "Issues",
                column: "AuthorIdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_NormalizedDescriptionUpper",
                table: "Issues",
                column: "NormalizedDescriptionUpper");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_NormalizedNameUpper",
                table: "Issues",
                column: "NormalizedNameUpper");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rubrics_ContextName",
                table: "Rubrics");

            migrationBuilder.DropIndex(
                name: "IX_Rubrics_NormalizedNameUpper",
                table: "Rubrics");

            migrationBuilder.DropIndex(
                name: "IX_Issues_AuthorIdentityUserId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_NormalizedDescriptionUpper",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_NormalizedNameUpper",
                table: "Issues");
        }
    }
}
