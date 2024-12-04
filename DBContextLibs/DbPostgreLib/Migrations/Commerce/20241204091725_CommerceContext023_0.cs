using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext023_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContextName",
                table: "Goods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedNameUpper",
                table: "Goods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Goods",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Goods",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "SortIndex",
                table: "Goods",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Goods_ContextName",
                table: "Goods",
                column: "ContextName");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_NormalizedNameUpper",
                table: "Goods",
                column: "NormalizedNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_SortIndex_ParentId",
                table: "Goods",
                columns: new[] { "SortIndex", "ParentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Goods_ContextName",
                table: "Goods");

            migrationBuilder.DropIndex(
                name: "IX_Goods_NormalizedNameUpper",
                table: "Goods");

            migrationBuilder.DropIndex(
                name: "IX_Goods_SortIndex_ParentId",
                table: "Goods");

            migrationBuilder.DropColumn(
                name: "ContextName",
                table: "Goods");

            migrationBuilder.DropColumn(
                name: "NormalizedNameUpper",
                table: "Goods");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Goods");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Goods");

            migrationBuilder.DropColumn(
                name: "SortIndex",
                table: "Goods");
        }
    }
}
