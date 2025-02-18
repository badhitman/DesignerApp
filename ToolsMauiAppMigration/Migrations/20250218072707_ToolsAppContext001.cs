using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolsMauiAppMigration.Migrations
{
    /// <inheritdoc />
    public partial class ToolsAppContext001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddressBaseUri = table.Column<string>(type: "TEXT", nullable: true),
                    TokenAccess = table.Column<string>(type: "TEXT", nullable: true),
                    HeaderName = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExeCommands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    Arguments = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExeCommands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExeCommands_Configurations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Configurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SyncDirectories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocalDirectory = table.Column<string>(type: "TEXT", nullable: true),
                    RemoteDirectory = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncDirectories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncDirectories_Configurations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Configurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_Name",
                table: "Configurations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ExeCommands_Name",
                table: "ExeCommands",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ExeCommands_ParentId",
                table: "ExeCommands",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncDirectories_Name",
                table: "SyncDirectories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SyncDirectories_ParentId",
                table: "SyncDirectories",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExeCommands");

            migrationBuilder.DropTable(
                name: "SyncDirectories");

            migrationBuilder.DropTable(
                name: "Configurations");
        }
    }
}
