using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilesTags");

            migrationBuilder.CreateTable(
                name: "CloudTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerPrimaryKey = table.Column<int>(type: "integer", nullable: false),
                    ContextName = table.Column<string>(type: "text", nullable: false),
                    NormalizedNameUpper = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudTags", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_Name",
                table: "CloudTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedNameUpper",
                table: "CloudTags",
                column: "NormalizedNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_CloudTags_NormalizedNameUpper_ContextName",
                table: "CloudTags",
                columns: new[] { "NormalizedNameUpper", "ContextName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloudTags");

            migrationBuilder.CreateTable(
                name: "FilesTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerFileId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedNameUpper = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilesTags_CloudFiles_OwnerFileId",
                        column: x => x.OwnerFileId,
                        principalTable: "CloudFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilesTags_Name",
                table: "FilesTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FilesTags_NormalizedNameUpper",
                table: "FilesTags",
                column: "NormalizedNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_FilesTags_NormalizedNameUpper_OwnerFileId",
                table: "FilesTags",
                columns: new[] { "NormalizedNameUpper", "OwnerFileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilesTags_OwnerFileId",
                table: "FilesTags",
                column: "OwnerFileId");
        }
    }
}
