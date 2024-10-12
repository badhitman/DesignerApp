using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RubricsForArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SortIndex = table.Column<long>(type: "bigint", nullable: false),
                    ParentRubricId = table.Column<int>(type: "integer", nullable: true),
                    ContextName = table.Column<string>(type: "text", nullable: true),
                    NormalizedNameUpper = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorIdentityId = table.Column<string>(type: "text", nullable: false),
                    RubricArticleModelDBId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_RubricsForArticles_RubricArticleModelDBId",
                        column: x => x.RubricArticleModelDBId,
                        principalTable: "RubricsForArticles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArticlesTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerArticleId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_Articles_AuthorIdentityId",
                table: "Articles",
                column: "AuthorIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CreatedAtUTC",
                table: "Articles",
                column: "CreatedAtUTC");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Name",
                table: "Articles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_RubricArticleModelDBId",
                table: "Articles",
                column: "RubricArticleModelDBId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UpdatedAtUTC",
                table: "Articles",
                column: "UpdatedAtUTC");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlesTags_Name",
                table: "ArticlesTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlesTags_OwnerArticleId",
                table: "ArticlesTags",
                column: "OwnerArticleId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticlesTags");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "RubricsForArticles");
        }
    }
}
