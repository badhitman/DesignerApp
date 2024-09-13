using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CloudProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SerializedDataJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeName = table.Column<string>(type: "text", nullable: false),
                    ApplicationName = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PrefixPropertyName = table.Column<string>(type: "text", nullable: true),
                    OwnerPrimaryKey = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudProperties", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CloudProperties_ApplicationName_Name",
                table: "CloudProperties",
                columns: new[] { "ApplicationName", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CloudProperties_CreatedAt",
                table: "CloudProperties",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CloudProperties_PrefixPropertyName_OwnerPrimaryKey",
                table: "CloudProperties",
                columns: new[] { "PrefixPropertyName", "OwnerPrimaryKey" });

            migrationBuilder.CreateIndex(
                name: "IX_CloudProperties_TypeName",
                table: "CloudProperties",
                column: "TypeName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloudProperties");
        }
    }
}
