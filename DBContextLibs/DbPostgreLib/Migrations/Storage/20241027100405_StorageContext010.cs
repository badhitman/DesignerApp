using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedTagNameUpper_OwnerPrimaryKey",
                table: "CloudTags");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedTagNameUpper_OwnerPrimaryKey_Applicatio~",
                table: "CloudTags",
                columns: new[] { "NormalizedTagNameUpper", "OwnerPrimaryKey", "ApplicationName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedTagNameUpper_OwnerPrimaryKey_Applicatio~",
                table: "CloudTags");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedTagNameUpper_OwnerPrimaryKey",
                table: "CloudTags",
                columns: new[] { "NormalizedTagNameUpper", "OwnerPrimaryKey" },
                unique: true);
        }
    }
}
