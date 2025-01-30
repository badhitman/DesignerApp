using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.NLogs
{
    /// <inheritdoc />
    public partial class NLogsContext001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationName = table.Column<string>(type: "text", nullable: false),
                    ContextPrefix = table.Column<string>(type: "text", nullable: true),
                    RecordTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecordLevel = table.Column<string>(type: "text", nullable: false),
                    RecordMessage = table.Column<string>(type: "text", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "text", nullable: true),
                    Logger = table.Column<string>(type: "text", nullable: true),
                    CallSite = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    AllEventProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ApplicationName",
                table: "Logs",
                column: "ApplicationName");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ContextPrefix",
                table: "Logs",
                column: "ContextPrefix");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Logger",
                table: "Logs",
                column: "Logger");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_RecordLevel",
                table: "Logs",
                column: "RecordLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_RecordTime",
                table: "Logs",
                column: "RecordTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
