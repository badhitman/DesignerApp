﻿// <auto-generated />
using System;
using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Helpdesk
{
    [DbContext(typeof(HelpdeskContext))]
    [Migration("20241013164857_HelpdeskPostgreContext007")]
    partial class HelpdeskPostgreContext007
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SharedLib.AnonymTelegramAccessHelpdeskModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("TelegramUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("TokenAccess")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("TokenAccess")
                        .IsUnique();

                    b.ToTable("AccessTokens");
                });

            modelBuilder.Entity("SharedLib.AnswerToForwardModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ForwardMessageId")
                        .HasColumnType("integer");

                    b.Property<int>("ResultMessageId")
                        .HasColumnType("integer");

                    b.Property<int>("ResultMessageTelegramId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAtUtc");

                    b.HasIndex("ForwardMessageId");

                    b.HasIndex("ResultMessageId");

                    b.HasIndex("ResultMessageTelegramId");

                    b.ToTable("AnswersToForwards");
                });

            modelBuilder.Entity("SharedLib.ArticleModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthorIdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAtUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedNameUpper")
                        .HasColumnType("text");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<long>("SortIndex")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedAtUTC")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorIdentityId");

                    b.HasIndex("CreatedAtUTC");

                    b.HasIndex("Name");

                    b.HasIndex("UpdatedAtUTC");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("SharedLib.ArticleTagModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OwnerArticleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerArticleId");

                    b.ToTable("ArticlesTags");
                });

            modelBuilder.Entity("SharedLib.ForwardMessageTelegramBotModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("DestinationChatId")
                        .HasColumnType("bigint");

                    b.Property<int>("ResultMessageId")
                        .HasColumnType("integer");

                    b.Property<int>("ResultMessageTelegramId")
                        .HasColumnType("integer");

                    b.Property<long>("SourceChatId")
                        .HasColumnType("bigint");

                    b.Property<int>("SourceMessageId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAtUtc");

                    b.HasIndex("ResultMessageId");

                    b.HasIndex("ResultMessageTelegramId");

                    b.HasIndex("DestinationChatId", "SourceChatId", "SourceMessageId");

                    b.ToTable("ForwardedMessages");
                });

            modelBuilder.Entity("SharedLib.IssueHelpdeskModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthorIdentityUserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ExecutorIdentityUserId")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedDescriptionUpper")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedNameUpper")
                        .HasColumnType("text");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<int?>("RubricIssueId")
                        .HasColumnType("integer");

                    b.Property<int>("StepIssue")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("LastUpdateAt");

                    b.HasIndex("Name");

                    b.HasIndex("RubricIssueId");

                    b.HasIndex("AuthorIdentityUserId", "ExecutorIdentityUserId", "LastUpdateAt", "RubricIssueId", "StepIssue");

                    b.ToTable("Issues");
                });

            modelBuilder.Entity("SharedLib.IssueMessageHelpdeskModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthorUserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("IssueId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastUpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MessageText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorUserId");

                    b.HasIndex("IssueId");

                    b.HasIndex("CreatedAt", "LastUpdateAt");

                    b.ToTable("IssuesMessages");
                });

            modelBuilder.Entity("SharedLib.IssueReadMarkerHelpdeskModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("IssueId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastReadAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserIdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("IssueId");

                    b.HasIndex("LastReadAt", "UserIdentityId")
                        .IsUnique();

                    b.ToTable("IssueReadMarkers");
                });

            modelBuilder.Entity("SharedLib.LockUniqueTokenModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.ToTable("Lockers");
                });

            modelBuilder.Entity("SharedLib.PulseIssueModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthorUserIdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("IssueId")
                        .HasColumnType("integer");

                    b.Property<int>("PulseType")
                        .HasColumnType("integer");

                    b.Property<string>("Tag")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorUserIdentityId");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("IssueId");

                    b.HasIndex("PulseType");

                    b.HasIndex("Tag");

                    b.ToTable("PulseEvents");
                });

            modelBuilder.Entity("SharedLib.RubricArticleJoinModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<int>("RubricId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("RubricId");

                    b.ToTable("RubricsArticlesJoins");
                });

            modelBuilder.Entity("SharedLib.RubricIssueHelpdeskModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ContextName")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedNameUpper")
                        .HasColumnType("text");

                    b.Property<int?>("ParentRubricId")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<long>("SortIndex")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("IsDisabled");

                    b.HasIndex("Name");

                    b.HasIndex("ParentRubricId");

                    b.HasIndex("SortIndex", "ParentRubricId")
                        .IsUnique();

                    b.ToTable("RubricsForIssues");
                });

            modelBuilder.Entity("SharedLib.SubscriberIssueHelpdeskModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsSilent")
                        .HasColumnType("boolean");

                    b.Property<int>("IssueId")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("IssueId");

                    b.HasIndex("UserId", "IssueId")
                        .IsUnique();

                    b.ToTable("SubscribersOfIssues");
                });

            modelBuilder.Entity("SharedLib.VoteHelpdeskModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("IdentityUserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("IssueId")
                        .HasColumnType("integer");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IdentityUserId");

                    b.HasIndex("IssueId");

                    b.HasIndex("MessageId");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("SharedLib.AnswerToForwardModelDB", b =>
                {
                    b.HasOne("SharedLib.ForwardMessageTelegramBotModelDB", "ForwardMessage")
                        .WithMany("Answers")
                        .HasForeignKey("ForwardMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ForwardMessage");
                });

            modelBuilder.Entity("SharedLib.ArticleTagModelDB", b =>
                {
                    b.HasOne("SharedLib.ArticleModelDB", "OwnerArticle")
                        .WithMany("Tags")
                        .HasForeignKey("OwnerArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OwnerArticle");
                });

            modelBuilder.Entity("SharedLib.IssueHelpdeskModelDB", b =>
                {
                    b.HasOne("SharedLib.RubricIssueHelpdeskModelDB", "RubricIssue")
                        .WithMany("Issues")
                        .HasForeignKey("RubricIssueId");

                    b.Navigation("RubricIssue");
                });

            modelBuilder.Entity("SharedLib.IssueMessageHelpdeskModelDB", b =>
                {
                    b.HasOne("SharedLib.IssueHelpdeskModelDB", "Issue")
                        .WithMany("Messages")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Issue");
                });

            modelBuilder.Entity("SharedLib.IssueReadMarkerHelpdeskModelDB", b =>
                {
                    b.HasOne("SharedLib.IssueHelpdeskModelDB", "Issue")
                        .WithMany("ReadMarkers")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Issue");
                });

            modelBuilder.Entity("SharedLib.PulseIssueModelDB", b =>
                {
                    b.HasOne("SharedLib.IssueHelpdeskModelDB", "Issue")
                        .WithMany("PulseEvents")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Issue");
                });

            modelBuilder.Entity("SharedLib.RubricArticleJoinModelDB", b =>
                {
                    b.HasOne("SharedLib.ArticleModelDB", "Article")
                        .WithMany("RubricsJoins")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.RubricIssueHelpdeskModelDB", "Rubric")
                        .WithMany("ArticlesJoins")
                        .HasForeignKey("RubricId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Rubric");
                });

            modelBuilder.Entity("SharedLib.RubricIssueHelpdeskModelDB", b =>
                {
                    b.HasOne("SharedLib.RubricIssueHelpdeskModelDB", "ParentRubric")
                        .WithMany("NestedRubrics")
                        .HasForeignKey("ParentRubricId");

                    b.Navigation("ParentRubric");
                });

            modelBuilder.Entity("SharedLib.SubscriberIssueHelpdeskModelDB", b =>
                {
                    b.HasOne("SharedLib.IssueHelpdeskModelDB", "Issue")
                        .WithMany("Subscribers")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Issue");
                });

            modelBuilder.Entity("SharedLib.VoteHelpdeskModelDB", b =>
                {
                    b.HasOne("SharedLib.IssueHelpdeskModelDB", "Issue")
                        .WithMany()
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.IssueMessageHelpdeskModelDB", "Message")
                        .WithMany("Votes")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Issue");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SharedLib.ArticleModelDB", b =>
                {
                    b.Navigation("RubricsJoins");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("SharedLib.ForwardMessageTelegramBotModelDB", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("SharedLib.IssueHelpdeskModelDB", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("PulseEvents");

                    b.Navigation("ReadMarkers");

                    b.Navigation("Subscribers");
                });

            modelBuilder.Entity("SharedLib.IssueMessageHelpdeskModelDB", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("SharedLib.RubricIssueHelpdeskModelDB", b =>
                {
                    b.Navigation("ArticlesJoins");

                    b.Navigation("Issues");

                    b.Navigation("NestedRubrics");
                });
#pragma warning restore 612, 618
        }
    }
}
