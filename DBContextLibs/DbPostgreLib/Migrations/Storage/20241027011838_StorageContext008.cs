using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext008 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudTags_Name",
                table: "CloudTags");

            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedNameUpper_ContextName_OwnerPrimaryKey",
                table: "CloudTags");

            migrationBuilder.RenameColumn(
                name: "ContextName",
                table: "CloudTags",
                newName: "ApplicationName");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerPrimaryKey",
                table: "CloudTags",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CloudTags",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PrefixPropertyName",
                table: "CloudTags",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_ApplicationName_Name",
                table: "CloudTags",
                columns: new[] { "ApplicationName", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_CreatedAt",
                table: "CloudTags",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedNameUpper_OwnerPrimaryKey",
                table: "CloudTags",
                columns: new[] { "NormalizedNameUpper", "OwnerPrimaryKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_PrefixPropertyName_OwnerPrimaryKey",
                table: "CloudTags",
                columns: new[] { "PrefixPropertyName", "OwnerPrimaryKey" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudTags_ApplicationName_Name",
                table: "CloudTags");

            migrationBuilder.DropIndex(
                name: "IX_CloudTags_CreatedAt",
                table: "CloudTags");

            migrationBuilder.DropIndex(
                name: "IX_CloudTags_NormalizedNameUpper_OwnerPrimaryKey",
                table: "CloudTags");

            migrationBuilder.DropIndex(
                name: "IX_CloudTags_PrefixPropertyName_OwnerPrimaryKey",
                table: "CloudTags");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CloudTags");

            migrationBuilder.DropColumn(
                name: "PrefixPropertyName",
                table: "CloudTags");

            migrationBuilder.RenameColumn(
                name: "ApplicationName",
                table: "CloudTags",
                newName: "ContextName");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerPrimaryKey",
                table: "CloudTags",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_Name",
                table: "CloudTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedNameUpper_ContextName_OwnerPrimaryKey",
                table: "CloudTags",
                columns: new[] { "NormalizedNameUpper", "ContextName", "OwnerPrimaryKey" },
                unique: true);
        }
    }
}
