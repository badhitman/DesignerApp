using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext035 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkScheduleBaseModelDB_ExecutorIdentityUserId",
                table: "WorkScheduleBaseModelDB");

            migrationBuilder.DropColumn(
                name: "ExecutorIdentityUserId",
                table: "WorkScheduleBaseModelDB");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExecutorIdentityUserId",
                table: "WorkScheduleBaseModelDB",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkScheduleBaseModelDB_ExecutorIdentityUserId",
                table: "WorkScheduleBaseModelDB",
                column: "ExecutorIdentityUserId");
        }
    }
}
