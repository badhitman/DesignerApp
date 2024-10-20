using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Constructor
{
    /// <inheritdoc />
    public partial class ConstructorContext003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedUpperName",
                table: "Forms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_NormalizedUpperName",
                table: "Forms",
                column: "NormalizedUpperName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Forms_NormalizedUpperName",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "NormalizedUpperName",
                table: "Forms");
        }
    }
}
