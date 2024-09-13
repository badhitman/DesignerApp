using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    /// <inheritdoc />
    public partial class HelpdeskPostgreContext001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokenAccess = table.Column<string>(type: "text", nullable: false),
                    TelegramUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForwardedMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResultMessageTelegramId = table.Column<int>(type: "integer", nullable: false),
                    ResultMessageId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DestinationChatId = table.Column<long>(type: "bigint", nullable: false),
                    SourceChatId = table.Column<long>(type: "bigint", nullable: false),
                    SourceMessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForwardedMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lockers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lockers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RubricsForIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SortIndex = table.Column<long>(type: "bigint", nullable: false),
                    ParentRubricId = table.Column<int>(type: "integer", nullable: true),
                    ContextName = table.Column<string>(type: "text", nullable: true),
                    NormalizedNameUpper = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RubricsForIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RubricsForIssues_RubricsForIssues_ParentRubricId",
                        column: x => x.ParentRubricId,
                        principalTable: "RubricsForIssues",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AnswersToForwards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResultMessageTelegramId = table.Column<int>(type: "integer", nullable: false),
                    ResultMessageId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ForwardMessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswersToForwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswersToForwards_ForwardedMessages_ForwardMessageId",
                        column: x => x.ForwardMessageId,
                        principalTable: "ForwardedMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NormalizedNameUpper = table.Column<string>(type: "text", nullable: true),
                    RubricIssueId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StepIssue = table.Column<int>(type: "integer", nullable: false),
                    AuthorIdentityUserId = table.Column<string>(type: "text", nullable: false),
                    NormalizedDescriptionUpper = table.Column<string>(type: "text", nullable: true),
                    ExecutorIdentityUserId = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issues_RubricsForIssues_RubricIssueId",
                        column: x => x.RubricIssueId,
                        principalTable: "RubricsForIssues",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IssueReadMarkers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    LastReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserIdentityId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueReadMarkers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueReadMarkers_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssuesMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MessageText = table.Column<string>(type: "text", nullable: false),
                    IssueId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuesMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssuesMessages_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PulseEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorUserIdentityId = table.Column<string>(type: "text", nullable: false),
                    PulseType = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Tag = table.Column<string>(type: "text", nullable: true),
                    IssueId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PulseEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PulseEvents_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscribersOfIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    IsSilent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribersOfIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscribersOfIssues_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    IdentityUserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_IssuesMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "IssuesMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_CreatedAt",
                table: "AccessTokens",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_TokenAccess",
                table: "AccessTokens",
                column: "TokenAccess",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnswersToForwards_CreatedAtUtc",
                table: "AnswersToForwards",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AnswersToForwards_ForwardMessageId",
                table: "AnswersToForwards",
                column: "ForwardMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswersToForwards_ResultMessageId",
                table: "AnswersToForwards",
                column: "ResultMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswersToForwards_ResultMessageTelegramId",
                table: "AnswersToForwards",
                column: "ResultMessageTelegramId");

            migrationBuilder.CreateIndex(
                name: "IX_ForwardedMessages_CreatedAtUtc",
                table: "ForwardedMessages",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ForwardedMessages_DestinationChatId_SourceChatId_SourceMess~",
                table: "ForwardedMessages",
                columns: new[] { "DestinationChatId", "SourceChatId", "SourceMessageId" });

            migrationBuilder.CreateIndex(
                name: "IX_ForwardedMessages_ResultMessageId",
                table: "ForwardedMessages",
                column: "ResultMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ForwardedMessages_ResultMessageTelegramId",
                table: "ForwardedMessages",
                column: "ResultMessageTelegramId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReadMarkers_IssueId",
                table: "IssueReadMarkers",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReadMarkers_LastReadAt_UserIdentityId",
                table: "IssueReadMarkers",
                columns: new[] { "LastReadAt", "UserIdentityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AuthorIdentityUserId_ExecutorIdentityUserId_LastUpda~",
                table: "Issues",
                columns: new[] { "AuthorIdentityUserId", "ExecutorIdentityUserId", "LastUpdateAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_Name",
                table: "Issues",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_RubricIssueId",
                table: "Issues",
                column: "RubricIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuesMessages_AuthorUserId",
                table: "IssuesMessages",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuesMessages_CreatedAt_LastUpdateAt",
                table: "IssuesMessages",
                columns: new[] { "CreatedAt", "LastUpdateAt" });

            migrationBuilder.CreateIndex(
                name: "IX_IssuesMessages_IssueId",
                table: "IssuesMessages",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Lockers_Token",
                table: "Lockers",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PulseEvents_AuthorUserIdentityId",
                table: "PulseEvents",
                column: "AuthorUserIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_PulseEvents_CreatedAt",
                table: "PulseEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PulseEvents_IssueId",
                table: "PulseEvents",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_PulseEvents_PulseType",
                table: "PulseEvents",
                column: "PulseType");

            migrationBuilder.CreateIndex(
                name: "IX_PulseEvents_Tag",
                table: "PulseEvents",
                column: "Tag");

            migrationBuilder.CreateIndex(
                name: "IX_RubricsForIssues_IsDisabled",
                table: "RubricsForIssues",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_RubricsForIssues_Name",
                table: "RubricsForIssues",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RubricsForIssues_ParentRubricId",
                table: "RubricsForIssues",
                column: "ParentRubricId");

            migrationBuilder.CreateIndex(
                name: "IX_RubricsForIssues_SortIndex_ParentRubricId",
                table: "RubricsForIssues",
                columns: new[] { "SortIndex", "ParentRubricId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersOfIssues_IssueId",
                table: "SubscribersOfIssues",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribersOfIssues_UserId_IssueId",
                table: "SubscribersOfIssues",
                columns: new[] { "UserId", "IssueId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_IdentityUserId",
                table: "Votes",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_IssueId",
                table: "Votes",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_MessageId",
                table: "Votes",
                column: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessTokens");

            migrationBuilder.DropTable(
                name: "AnswersToForwards");

            migrationBuilder.DropTable(
                name: "IssueReadMarkers");

            migrationBuilder.DropTable(
                name: "Lockers");

            migrationBuilder.DropTable(
                name: "PulseEvents");

            migrationBuilder.DropTable(
                name: "SubscribersOfIssues");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "ForwardedMessages");

            migrationBuilder.DropTable(
                name: "IssuesMessages");

            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.DropTable(
                name: "RubricsForIssues");
        }
    }
}
