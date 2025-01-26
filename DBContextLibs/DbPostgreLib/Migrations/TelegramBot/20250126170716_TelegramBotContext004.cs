using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.TelegramBot
{
    /// <inheritdoc />
    public partial class TelegramBotContext004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParseModeName",
                table: "ErrorsSendingTextMessageTelegramBot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReplyToMessageId",
                table: "ErrorsSendingTextMessageTelegramBot",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignFrom",
                table: "ErrorsSendingTextMessageTelegramBot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceMessageId",
                table: "ErrorsSendingTextMessageTelegramBot",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParseModeName",
                table: "ErrorsSendingTextMessageTelegramBot");

            migrationBuilder.DropColumn(
                name: "ReplyToMessageId",
                table: "ErrorsSendingTextMessageTelegramBot");

            migrationBuilder.DropColumn(
                name: "SignFrom",
                table: "ErrorsSendingTextMessageTelegramBot");

            migrationBuilder.DropColumn(
                name: "SourceMessageId",
                table: "ErrorsSendingTextMessageTelegramBot");
        }
    }
}
