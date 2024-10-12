using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CloudFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileLength = table.Column<long>(type: "bigint", nullable: false),
                    AuthorIdentityId = table.Column<string>(type: "text", nullable: false),
                    PointId = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ReferrerMain = table.Column<string>(type: "text", nullable: true),
                    ApplicationName = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PrefixPropertyName = table.Column<string>(type: "text", nullable: true),
                    OwnerPrimaryKey = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilesTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerFileId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_CloudFiles_ApplicationName_Name",
                table: "CloudFiles",
                columns: new[] { "ApplicationName", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CloudFiles_AuthorIdentityId",
                table: "CloudFiles",
                column: "AuthorIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudFiles_CreatedAt",
                table: "CloudFiles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CloudFiles_FileName",
                table: "CloudFiles",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_CloudFiles_PointId",
                table: "CloudFiles",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudFiles_PrefixPropertyName_OwnerPrimaryKey",
                table: "CloudFiles",
                columns: new[] { "PrefixPropertyName", "OwnerPrimaryKey" });

            migrationBuilder.CreateIndex(
                name: "IX_CloudFiles_ReferrerMain",
                table: "CloudFiles",
                column: "ReferrerMain");

            migrationBuilder.CreateIndex(
                name: "IX_FilesTags_Name",
                table: "FilesTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FilesTags_OwnerFileId",
                table: "FilesTags",
                column: "OwnerFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilesTags");

            migrationBuilder.DropTable(
                name: "CloudFiles");
        }
    }
}
