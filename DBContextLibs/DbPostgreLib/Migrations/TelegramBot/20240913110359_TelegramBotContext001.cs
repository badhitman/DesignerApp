using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.TelegramBot
{
    /// <inheritdoc />
    public partial class TelegramBotContext001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatTelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    NormalizedTitleUpper = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: true),
                    NormalizedUsernameUpper = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    NormalizedFirstNameUpper = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    NormalizedLastNameUpper = table.Column<string>(type: "text", nullable: true),
                    IsForum = table.Column<bool>(type: "boolean", nullable: true),
                    LastUpdateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastMessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ErrorsSendingTextMessageTelegramBot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    ExceptionTypeName = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsEditing = table.Column<bool>(type: "boolean", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorsSendingTextMessageTelegramBot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserTelegramId = table.Column<long>(type: "bigint", nullable: false),
                    IsBot = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    NormalizedFirstNameUpper = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    NormalizedLastNameUpper = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: true),
                    NormalizedUsernameUpper = table.Column<string>(type: "text", nullable: true),
                    LanguageCode = table.Column<string>(type: "text", nullable: true),
                    IsPremium = table.Column<bool>(type: "boolean", nullable: true),
                    AddedToAttachmentMenu = table.Column<bool>(type: "boolean", nullable: true),
                    LastUpdateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastMessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatsPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SmallFileId = table.Column<string>(type: "text", nullable: false),
                    SmallFileUniqueId = table.Column<string>(type: "text", nullable: false),
                    BigFileId = table.Column<string>(type: "text", nullable: false),
                    BigFileUniqueId = table.Column<string>(type: "text", nullable: false),
                    ChatOwnerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatsPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatsPhotos_Chats_ChatOwnerId",
                        column: x => x.ChatOwnerId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinsUsersToChats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ChatId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinsUsersToChats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinsUsersToChats_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinsUsersToChats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageTelegramId = table.Column<int>(type: "integer", nullable: false),
                    MessageThreadId = table.Column<int>(type: "integer", nullable: true),
                    FromId = table.Column<int>(type: "integer", nullable: true),
                    ChatId = table.Column<int>(type: "integer", nullable: false),
                    SenderChatId = table.Column<int>(type: "integer", nullable: true),
                    ForwardFromId = table.Column<long>(type: "bigint", nullable: true),
                    IsTopicMessage = table.Column<bool>(type: "boolean", nullable: true),
                    ForwardFromChatId = table.Column<long>(type: "bigint", nullable: true),
                    ForwardFromMessageId = table.Column<int>(type: "integer", nullable: true),
                    ForwardSignature = table.Column<string>(type: "text", nullable: true),
                    ForwardSenderName = table.Column<string>(type: "text", nullable: true),
                    ForwardDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAutomaticForward = table.Column<bool>(type: "boolean", nullable: true),
                    ReplyToMessageId = table.Column<int>(type: "integer", nullable: true),
                    ViaBotId = table.Column<long>(type: "bigint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MediaGroupId = table.Column<string>(type: "text", nullable: true),
                    AuthorSignature = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true),
                    NormalizedTextUpper = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AudioId = table.Column<int>(type: "integer", nullable: true),
                    VideoId = table.Column<int>(type: "integer", nullable: true),
                    DocumentId = table.Column<int>(type: "integer", nullable: true),
                    VoiceId = table.Column<int>(type: "integer", nullable: true),
                    ContactId = table.Column<int>(type: "integer", nullable: true),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    NormalizedCaptionUpper = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Users_FromId",
                        column: x => x.FromId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Audios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Performer = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    MimeType = table.Column<string>(type: "text", nullable: true),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileUniqueId = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audios_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Vcard = table.Column<string>(type: "text", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    MimeType = table.Column<string>(type: "text", nullable: true),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileUniqueId = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotosMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileUniqueId = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotosMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotosMessages_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    MimeType = table.Column<string>(type: "text", nullable: true),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileUniqueId = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Voices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    MimeType = table.Column<string>(type: "text", nullable: true),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileUniqueId = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voices_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AudiosThumbnails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AudioOwnerId = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileUniqueId = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudiosThumbnails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudiosThumbnails_Audios_AudioOwnerId",
                        column: x => x.AudioOwnerId,
                        principalTable: "Audios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudiosThumbnails_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsThumbnails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentOwnerId = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileUniqueId = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsThumbnails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsThumbnails_Documents_DocumentOwnerId",
                        column: x => x.DocumentOwnerId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentsThumbnails_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideosThumbnails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VideoOwnerId = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileUniqueId = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosThumbnails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideosThumbnails_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideosThumbnails_Videos_VideoOwnerId",
                        column: x => x.VideoOwnerId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audios_MessageId",
                table: "Audios",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudiosThumbnails_AudioOwnerId",
                table: "AudiosThumbnails",
                column: "AudioOwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudiosThumbnails_MessageId",
                table: "AudiosThumbnails",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ChatTelegramId",
                table: "Chats",
                column: "ChatTelegramId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_FirstName",
                table: "Chats",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_IsForum",
                table: "Chats",
                column: "IsForum");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastName",
                table: "Chats",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_Title",
                table: "Chats",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_Type",
                table: "Chats",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_Username",
                table: "Chats",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsPhotos_ChatOwnerId",
                table: "ChatsPhotos",
                column: "ChatOwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_MessageId",
                table: "Contacts",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_MessageId",
                table: "Documents",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsThumbnails_DocumentOwnerId",
                table: "DocumentsThumbnails",
                column: "DocumentOwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsThumbnails_MessageId",
                table: "DocumentsThumbnails",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorsSendingTextMessageTelegramBot_ChatId",
                table: "ErrorsSendingTextMessageTelegramBot",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorsSendingTextMessageTelegramBot_IsDisabled",
                table: "ErrorsSendingTextMessageTelegramBot",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_JoinsUsersToChats_ChatId",
                table: "JoinsUsersToChats",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinsUsersToChats_UserId_ChatId",
                table: "JoinsUsersToChats",
                columns: new[] { "UserId", "ChatId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_FromId",
                table: "Messages",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageTelegramId_ChatId_FromId",
                table: "Messages",
                columns: new[] { "MessageTelegramId", "ChatId", "FromId" });

            migrationBuilder.CreateIndex(
                name: "IX_PhotosMessages_MessageId",
                table: "PhotosMessages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName",
                table: "Users",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsBot",
                table: "Users",
                column: "IsBot");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastName",
                table: "Users",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTelegramId",
                table: "Users",
                column: "UserTelegramId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_MessageId",
                table: "Videos",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideosThumbnails_MessageId",
                table: "VideosThumbnails",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosThumbnails_VideoOwnerId",
                table: "VideosThumbnails",
                column: "VideoOwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voices_MessageId",
                table: "Voices",
                column: "MessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudiosThumbnails");

            migrationBuilder.DropTable(
                name: "ChatsPhotos");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "DocumentsThumbnails");

            migrationBuilder.DropTable(
                name: "ErrorsSendingTextMessageTelegramBot");

            migrationBuilder.DropTable(
                name: "JoinsUsersToChats");

            migrationBuilder.DropTable(
                name: "PhotosMessages");

            migrationBuilder.DropTable(
                name: "VideosThumbnails");

            migrationBuilder.DropTable(
                name: "Voices");

            migrationBuilder.DropTable(
                name: "Audios");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
