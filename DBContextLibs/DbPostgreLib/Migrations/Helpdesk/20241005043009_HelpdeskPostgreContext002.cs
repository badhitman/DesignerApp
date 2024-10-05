using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Issues_AuthorIdentityUserId_ExecutorIdentityUserId_LastUpda~",
                table: "Issues");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AuthorIdentityUserId_ExecutorIdentityUserId_LastUpda~",
                table: "Issues",
                columns: new[] { "AuthorIdentityUserId", "ExecutorIdentityUserId", "LastUpdateAt", "RubricIssueId", "StepIssue" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_CreatedAt",
                table: "Issues",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_LastUpdateAt",
                table: "Issues",
                column: "LastUpdateAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Issues_AuthorIdentityUserId_ExecutorIdentityUserId_LastUpda~",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_CreatedAt",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_LastUpdateAt",
                table: "Issues");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AuthorIdentityUserId_ExecutorIdentityUserId_LastUpda~",
                table: "Issues",
                columns: new[] { "AuthorIdentityUserId", "ExecutorIdentityUserId", "LastUpdateAt" });
        }
    }
}
