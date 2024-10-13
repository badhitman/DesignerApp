using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NormalizedNameUpper",
                table: "CloudFiles",
                newName: "NormalizedFileNameUpper");

            migrationBuilder.RenameIndex(
                name: "IX_CloudFiles_NormalizedNameUpper",
                table: "CloudFiles",
                newName: "IX_CloudFiles_NormalizedFileNameUpper");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NormalizedFileNameUpper",
                table: "CloudFiles",
                newName: "NormalizedNameUpper");

            migrationBuilder.RenameIndex(
                name: "IX_CloudFiles_NormalizedFileNameUpper",
                table: "CloudFiles",
                newName: "IX_CloudFiles_NormalizedNameUpper");
        }
    }
}
