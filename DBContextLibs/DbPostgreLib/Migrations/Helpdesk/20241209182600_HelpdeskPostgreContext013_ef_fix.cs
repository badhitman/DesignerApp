using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext013_ef_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rubrics_SortIndex_ParentId",
                table: "Rubrics");

            migrationBuilder.CreateIndex(
                name: "IX_Rubrics_SortIndex_ParentId_ContextName",
                table: "Rubrics",
                columns: new[] { "SortIndex", "ParentId", "ContextName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rubrics_SortIndex_ParentId_ContextName",
                table: "Rubrics");

            migrationBuilder.CreateIndex(
                name: "IX_Rubrics_SortIndex_ParentId",
                table: "Rubrics",
                columns: new[] { "SortIndex", "ParentId" },
                unique: true);
        }
    }
}
