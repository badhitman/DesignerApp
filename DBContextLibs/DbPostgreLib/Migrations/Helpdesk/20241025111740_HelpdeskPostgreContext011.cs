using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticlesTags");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_StepIssue",
                table: "Issues",
                column: "StepIssue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Issues_StepIssue",
                table: "Issues");

            migrationBuilder.CreateTable(
                name: "ArticlesTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerArticleId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedNameUpper = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticlesTags_Articles_OwnerArticleId",
                        column: x => x.OwnerArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticlesTags_Name",
                table: "ArticlesTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlesTags_NormalizedNameUpper",
                table: "ArticlesTags",
                column: "NormalizedNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlesTags_OwnerArticleId",
                table: "ArticlesTags",
                column: "OwnerArticleId");
        }
    }
}
