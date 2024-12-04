using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext013 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rubrics_Rubrics_ParentRubricId",
                table: "Rubrics");

            migrationBuilder.RenameColumn(
                name: "ParentRubricId",
                table: "Rubrics",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Rubrics_SortIndex_ParentRubricId",
                table: "Rubrics",
                newName: "IX_Rubrics_SortIndex_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Rubrics_ParentRubricId",
                table: "Rubrics",
                newName: "IX_Rubrics_ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rubrics_Rubrics_ParentId",
                table: "Rubrics",
                column: "ParentId",
                principalTable: "Rubrics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rubrics_Rubrics_ParentId",
                table: "Rubrics");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Rubrics",
                newName: "ParentRubricId");

            migrationBuilder.RenameIndex(
                name: "IX_Rubrics_SortIndex_ParentId",
                table: "Rubrics",
                newName: "IX_Rubrics_SortIndex_ParentRubricId");

            migrationBuilder.RenameIndex(
                name: "IX_Rubrics_ParentId",
                table: "Rubrics",
                newName: "IX_Rubrics_ParentRubricId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rubrics_Rubrics_ParentRubricId",
                table: "Rubrics",
                column: "ParentRubricId",
                principalTable: "Rubrics",
                principalColumn: "Id");
        }
    }
}
