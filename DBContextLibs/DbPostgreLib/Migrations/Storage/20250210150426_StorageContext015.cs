using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    /// <inheritdoc />
    public partial class StorageContext015 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AltnamesKLADR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OLDCODE = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    NEWCODE = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    LEVEL = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AltnamesKLADR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NamesMapsKLADR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SHNAME = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    SCNAME = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NAME = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CODE = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamesMapsKLADR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObjectsKLADR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    STATUS = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    NAME = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CODE = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    SOCR = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    INDEX = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    GNINMB = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    UNO = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    OCATD = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectsKLADR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocrbaseKLADRModelDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LEVEL = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    SCNAME = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SOCRNAME = table.Column<string>(type: "character varying(29)", maxLength: 29, nullable: false),
                    KOD_T_ST = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocrbaseKLADRModelDB", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AltnamesKLADR_LEVEL",
                table: "AltnamesKLADR",
                column: "LEVEL");

            migrationBuilder.CreateIndex(
                name: "IX_AltnamesKLADR_NEWCODE",
                table: "AltnamesKLADR",
                column: "NEWCODE");

            migrationBuilder.CreateIndex(
                name: "IX_AltnamesKLADR_OLDCODE",
                table: "AltnamesKLADR",
                column: "OLDCODE");

            migrationBuilder.CreateIndex(
                name: "IX_NamesMapsKLADR_CODE",
                table: "NamesMapsKLADR",
                column: "CODE");

            migrationBuilder.CreateIndex(
                name: "IX_NamesMapsKLADR_NAME",
                table: "NamesMapsKLADR",
                column: "NAME");

            migrationBuilder.CreateIndex(
                name: "IX_NamesMapsKLADR_SCNAME",
                table: "NamesMapsKLADR",
                column: "SCNAME");

            migrationBuilder.CreateIndex(
                name: "IX_NamesMapsKLADR_SHNAME",
                table: "NamesMapsKLADR",
                column: "SHNAME");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsKLADR_CODE",
                table: "ObjectsKLADR",
                column: "CODE");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsKLADR_GNINMB",
                table: "ObjectsKLADR",
                column: "GNINMB");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsKLADR_INDEX",
                table: "ObjectsKLADR",
                column: "INDEX");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsKLADR_NAME",
                table: "ObjectsKLADR",
                column: "NAME");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsKLADR_OCATD",
                table: "ObjectsKLADR",
                column: "OCATD");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsKLADR_SOCR",
                table: "ObjectsKLADR",
                column: "SOCR");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsKLADR_STATUS",
                table: "ObjectsKLADR",
                column: "STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsKLADR_UNO",
                table: "ObjectsKLADR",
                column: "UNO");

            migrationBuilder.CreateIndex(
                name: "IX_SocrbaseKLADRModelDB_KOD_T_ST",
                table: "SocrbaseKLADRModelDB",
                column: "KOD_T_ST");

            migrationBuilder.CreateIndex(
                name: "IX_SocrbaseKLADRModelDB_LEVEL",
                table: "SocrbaseKLADRModelDB",
                column: "LEVEL");

            migrationBuilder.CreateIndex(
                name: "IX_SocrbaseKLADRModelDB_SCNAME",
                table: "SocrbaseKLADRModelDB",
                column: "SCNAME");

            migrationBuilder.CreateIndex(
                name: "IX_SocrbaseKLADRModelDB_SOCRNAME",
                table: "SocrbaseKLADRModelDB",
                column: "SOCRNAME");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AltnamesKLADR");

            migrationBuilder.DropTable(
                name: "NamesMapsKLADR");

            migrationBuilder.DropTable(
                name: "ObjectsKLADR");

            migrationBuilder.DropTable(
                name: "SocrbaseKLADRModelDB");
        }
    }
}
