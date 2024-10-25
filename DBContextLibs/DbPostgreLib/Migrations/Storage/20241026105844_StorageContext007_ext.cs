using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext007_ext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedNameUpper_ContextName",
                table: "CloudTags");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedNameUpper_ContextName_OwnerPrimaryKey",
                table: "CloudTags",
                columns: new[] { "NormalizedNameUpper", "ContextName", "OwnerPrimaryKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedNameUpper_ContextName_OwnerPrimaryKey",
                table: "CloudTags");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedNameUpper_ContextName",
                table: "CloudTags",
                columns: new[] { "NormalizedNameUpper", "ContextName" },
                unique: true);
        }
    }
}
