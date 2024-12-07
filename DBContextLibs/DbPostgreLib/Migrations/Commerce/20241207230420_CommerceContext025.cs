using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_SortIndex_ParentId",
                table: "Nomenclatures");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_SortIndex_ParentId_ContextName",
                table: "Nomenclatures",
                columns: new[] { "SortIndex", "ParentId", "ContextName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_SortIndex_ParentId_ContextName",
                table: "Nomenclatures");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_SortIndex_ParentId",
                table: "Nomenclatures",
                columns: new[] { "SortIndex", "ParentId" },
                unique: true);
        }
    }
}
