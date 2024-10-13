using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RubricsArticlesJoins_RubricsForArticles_RubricId",
                table: "RubricsArticlesJoins");

            migrationBuilder.DropTable(
                name: "RubricsForArticles");

            migrationBuilder.AddForeignKey(
                name: "FK_RubricsArticlesJoins_RubricsForIssues_RubricId",
                table: "RubricsArticlesJoins",
                column: "RubricId",
                principalTable: "RubricsForIssues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RubricsArticlesJoins_RubricsForIssues_RubricId",
                table: "RubricsArticlesJoins");

            migrationBuilder.CreateTable(
                name: "RubricsForArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentRubricId = table.Column<int>(type: "integer", nullable: true),
                    ContextName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedNameUpper = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SortIndex = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RubricsForArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RubricsForArticles_RubricsForArticles_ParentRubricId",
                        column: x => x.ParentRubricId,
                        principalTable: "RubricsForArticles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RubricsForArticles_IsDisabled",
                table: "RubricsForArticles",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_RubricsForArticles_Name",
                table: "RubricsForArticles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RubricsForArticles_ParentRubricId",
                table: "RubricsForArticles",
                column: "ParentRubricId");

            migrationBuilder.CreateIndex(
                name: "IX_RubricsForArticles_SortIndex_ParentRubricId",
                table: "RubricsForArticles",
                columns: new[] { "SortIndex", "ParentRubricId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RubricsArticlesJoins_RubricsForArticles_RubricId",
                table: "RubricsArticlesJoins",
                column: "RubricId",
                principalTable: "RubricsForArticles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
