using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext009 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedNameUpper",
                table: "CloudTags");

            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedNameUpper_OwnerPrimaryKey",
                table: "CloudTags");

            migrationBuilder.RenameColumn(
                name: "NormalizedNameUpper",
                table: "CloudTags",
                newName: "TagName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CloudTags",
                newName: "PropertyName");

            migrationBuilder.RenameIndex(
                name: "IX_CloudTags_ApplicationName_Name",
                table: "CloudTags",
                newName: "IX_CloudTags_ApplicationName_PropertyName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CloudProperties",
                newName: "PropertyName");

            migrationBuilder.RenameIndex(
                name: "IX_CloudProperties_ApplicationName_Name",
                table: "CloudProperties",
                newName: "IX_CloudProperties_ApplicationName_PropertyName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CloudFiles",
                newName: "PropertyName");

            migrationBuilder.RenameIndex(
                name: "IX_CloudFiles_ApplicationName_Name",
                table: "CloudFiles",
                newName: "IX_CloudFiles_ApplicationName_PropertyName");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedTagNameUpper",
                table: "CloudTags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedTagNameUpper",
                table: "CloudTags",
                column: "NormalizedTagNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedTagNameUpper_OwnerPrimaryKey",
                table: "CloudTags",
                columns: new[] { "NormalizedTagNameUpper", "OwnerPrimaryKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedTagNameUpper",
                table: "CloudTags");

            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedTagNameUpper_OwnerPrimaryKey",
                table: "CloudTags");

            migrationBuilder.DropColumn(
                name: "NormalizedTagNameUpper",
                table: "CloudTags");

            migrationBuilder.RenameColumn(
                name: "TagName",
                table: "CloudTags",
                newName: "NormalizedNameUpper");

            migrationBuilder.RenameColumn(
                name: "PropertyName",
                table: "CloudTags",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_CloudTags_ApplicationName_PropertyName",
                table: "CloudTags",
                newName: "IX_CloudTags_ApplicationName_Name");

            migrationBuilder.RenameColumn(
                name: "PropertyName",
                table: "CloudProperties",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_CloudProperties_ApplicationName_PropertyName",
                table: "CloudProperties",
                newName: "IX_CloudProperties_ApplicationName_Name");

            migrationBuilder.RenameColumn(
                name: "PropertyName",
                table: "CloudFiles",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_CloudFiles_ApplicationName_PropertyName",
                table: "CloudFiles",
                newName: "IX_CloudFiles_ApplicationName_Name");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedNameUpper",
                table: "CloudTags",
                column: "NormalizedNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedNameUpper_OwnerPrimaryKey",
                table: "CloudTags",
                columns: new[] { "NormalizedNameUpper", "OwnerPrimaryKey" },
                unique: true);
        }
    }
}
