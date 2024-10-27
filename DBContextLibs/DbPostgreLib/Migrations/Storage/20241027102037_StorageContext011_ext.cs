using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext011_ext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TagNameOwnerKeyUnique",
                table: "CloudTags",
                columns: new[] { "NormalizedTagNameUpper", "OwnerPrimaryKey", "ApplicationName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TagNameOwnerKeyUnique",
                table: "CloudTags");
        }
    }
}
