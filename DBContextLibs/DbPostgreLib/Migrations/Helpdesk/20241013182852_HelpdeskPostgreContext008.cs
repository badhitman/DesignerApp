using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext008 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedNameUpper",
                table: "ArticlesTags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlesTags_NormalizedNameUpper",
                table: "ArticlesTags",
                column: "NormalizedNameUpper");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArticlesTags_NormalizedNameUpper",
                table: "ArticlesTags");

            migrationBuilder.DropColumn(
                name: "NormalizedNameUpper",
                table: "ArticlesTags");
        }
    }
}
