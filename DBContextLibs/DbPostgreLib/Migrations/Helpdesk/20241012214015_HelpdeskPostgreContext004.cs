using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_RubricsForArticles_RubricArticleModelDBId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_RubricArticleModelDBId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "RubricArticleModelDBId",
                table: "Articles");

            migrationBuilder.CreateTable(
                name: "RubricsArticlesJoins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RubricId = table.Column<int>(type: "integer", nullable: false),
                    ArticleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RubricsArticlesJoins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RubricsArticlesJoins_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RubricsArticlesJoins_RubricsForArticles_RubricId",
                        column: x => x.RubricId,
                        principalTable: "RubricsForArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RubricsArticlesJoins_ArticleId",
                table: "RubricsArticlesJoins",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_RubricsArticlesJoins_RubricId",
                table: "RubricsArticlesJoins",
                column: "RubricId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RubricsArticlesJoins");

            migrationBuilder.AddColumn<int>(
                name: "RubricArticleModelDBId",
                table: "Articles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_RubricArticleModelDBId",
                table: "Articles",
                column: "RubricArticleModelDBId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_RubricsForArticles_RubricArticleModelDBId",
                table: "Articles",
                column: "RubricArticleModelDBId",
                principalTable: "RubricsForArticles",
                principalColumn: "Id");
        }
    }
}
