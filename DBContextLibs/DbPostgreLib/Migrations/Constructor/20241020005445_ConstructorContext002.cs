using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Constructor
{
    /// <inheritdoc />
    public partial class ConstructorContext002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedUpperName",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ContextName",
                table: "Projects",
                column: "ContextName");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_NormalizedUpperName",
                table: "Projects",
                column: "NormalizedUpperName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ContextName",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_NormalizedUpperName",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "NormalizedUpperName",
                table: "Projects");
        }
    }
}
