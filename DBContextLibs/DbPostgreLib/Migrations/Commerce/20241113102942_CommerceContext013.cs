using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext013 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedUpperName",
                table: "WarehouseDocuments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseDocuments_NormalizedUpperName",
                table: "WarehouseDocuments",
                column: "NormalizedUpperName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WarehouseDocuments_NormalizedUpperName",
                table: "WarehouseDocuments");

            migrationBuilder.DropColumn(
                name: "NormalizedUpperName",
                table: "WarehouseDocuments");
        }
    }
}
