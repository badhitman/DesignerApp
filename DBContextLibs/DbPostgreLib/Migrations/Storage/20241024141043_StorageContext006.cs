using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FilesTags_NormalizedNameUpper_OwnerFileId",
                table: "FilesTags",
                columns: new[] { "NormalizedNameUpper", "OwnerFileId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilesTags_NormalizedNameUpper_OwnerFileId",
                table: "FilesTags");
        }
    }
}
