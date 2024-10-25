using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.TelegramBot
{
    /// <inheritdoc />
    public partial class TelegramBotContext002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastUpdateUtc",
                table: "Chats",
                column: "LastUpdateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_NormalizedFirstNameUpper",
                table: "Chats",
                column: "NormalizedFirstNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_NormalizedLastNameUpper",
                table: "Chats",
                column: "NormalizedLastNameUpper");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_NormalizedTitleUpper",
                table: "Chats",
                column: "NormalizedTitleUpper");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_NormalizedUsernameUpper",
                table: "Chats",
                column: "NormalizedUsernameUpper");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chats_LastUpdateUtc",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_NormalizedFirstNameUpper",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_NormalizedLastNameUpper",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_NormalizedTitleUpper",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_NormalizedUsernameUpper",
                table: "Chats");
        }
    }
}
