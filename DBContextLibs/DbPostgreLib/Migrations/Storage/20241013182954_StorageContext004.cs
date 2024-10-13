using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedNameUpper",
                table: "FilesTags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedNameUpper",
                table: "CloudFiles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilesTags_NormalizedNameUpper",
                table: "FilesTags",
                column: "NormalizedNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_CloudFiles_NormalizedNameUpper",
                table: "CloudFiles",
                column: "NormalizedNameUpper");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilesTags_NormalizedNameUpper",
                table: "FilesTags");

            migrationBuilder.DropIndex(
                name: "IX_CloudFiles_NormalizedNameUpper",
                table: "CloudFiles");

            migrationBuilder.DropColumn(
                name: "NormalizedNameUpper",
                table: "FilesTags");

            migrationBuilder.DropColumn(
                name: "NormalizedNameUpper",
                table: "CloudFiles");
        }
    }
}
