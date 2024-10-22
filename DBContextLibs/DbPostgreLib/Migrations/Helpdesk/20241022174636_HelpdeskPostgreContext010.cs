using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_RubricsForIssues_RubricIssueId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_RubricsArticlesJoins_RubricsForIssues_RubricId",
                table: "RubricsArticlesJoins");

            migrationBuilder.DropForeignKey(
                name: "FK_RubricsForIssues_RubricsForIssues_ParentRubricId",
                table: "RubricsForIssues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RubricsForIssues",
                table: "RubricsForIssues");

            migrationBuilder.RenameTable(
                name: "RubricsForIssues",
                newName: "Rubrics");

            migrationBuilder.RenameIndex(
                name: "IX_RubricsForIssues_SortIndex_ParentRubricId",
                table: "Rubrics",
                newName: "IX_Rubrics_SortIndex_ParentRubricId");

            migrationBuilder.RenameIndex(
                name: "IX_RubricsForIssues_ParentRubricId",
                table: "Rubrics",
                newName: "IX_Rubrics_ParentRubricId");

            migrationBuilder.RenameIndex(
                name: "IX_RubricsForIssues_Name",
                table: "Rubrics",
                newName: "IX_Rubrics_Name");

            migrationBuilder.RenameIndex(
                name: "IX_RubricsForIssues_IsDisabled",
                table: "Rubrics",
                newName: "IX_Rubrics_IsDisabled");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rubrics",
                table: "Rubrics",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Rubrics_RubricIssueId",
                table: "Issues",
                column: "RubricIssueId",
                principalTable: "Rubrics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rubrics_Rubrics_ParentRubricId",
                table: "Rubrics",
                column: "ParentRubricId",
                principalTable: "Rubrics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RubricsArticlesJoins_Rubrics_RubricId",
                table: "RubricsArticlesJoins",
                column: "RubricId",
                principalTable: "Rubrics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Rubrics_RubricIssueId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Rubrics_Rubrics_ParentRubricId",
                table: "Rubrics");

            migrationBuilder.DropForeignKey(
                name: "FK_RubricsArticlesJoins_Rubrics_RubricId",
                table: "RubricsArticlesJoins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rubrics",
                table: "Rubrics");

            migrationBuilder.RenameTable(
                name: "Rubrics",
                newName: "RubricsForIssues");

            migrationBuilder.RenameIndex(
                name: "IX_Rubrics_SortIndex_ParentRubricId",
                table: "RubricsForIssues",
                newName: "IX_RubricsForIssues_SortIndex_ParentRubricId");

            migrationBuilder.RenameIndex(
                name: "IX_Rubrics_ParentRubricId",
                table: "RubricsForIssues",
                newName: "IX_RubricsForIssues_ParentRubricId");

            migrationBuilder.RenameIndex(
                name: "IX_Rubrics_Name",
                table: "RubricsForIssues",
                newName: "IX_RubricsForIssues_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Rubrics_IsDisabled",
                table: "RubricsForIssues",
                newName: "IX_RubricsForIssues_IsDisabled");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RubricsForIssues",
                table: "RubricsForIssues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_RubricsForIssues_RubricIssueId",
                table: "Issues",
                column: "RubricIssueId",
                principalTable: "RubricsForIssues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RubricsArticlesJoins_RubricsForIssues_RubricId",
                table: "RubricsArticlesJoins",
                column: "RubricId",
                principalTable: "RubricsForIssues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RubricsForIssues_RubricsForIssues_ParentRubricId",
                table: "RubricsForIssues",
                column: "ParentRubricId",
                principalTable: "RubricsForIssues",
                principalColumn: "Id");
        }
    }
}
