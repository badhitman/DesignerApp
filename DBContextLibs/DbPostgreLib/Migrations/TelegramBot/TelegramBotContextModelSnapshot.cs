﻿// <auto-generated />
using System;
using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.TelegramBot
{
    [DbContext(typeof(TelegramBotContext))]
    partial class TelegramBotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SharedLib.AudioTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<long?>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<string>("MimeType")
                        .HasColumnType("text");

                    b.Property<string>("Performer")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.ToTable("Audios");
                });

            modelBuilder.Entity("SharedLib.AudioThumbnailTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AudioOwnerId")
                        .HasColumnType("integer");

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AudioOwnerId")
                        .IsUnique();

                    b.HasIndex("MessageId");

                    b.ToTable("AudiosThumbnails");
                });

            modelBuilder.Entity("SharedLib.ChatPhotoTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BigFileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BigFileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ChatOwnerId")
                        .HasColumnType("integer");

                    b.Property<string>("SmallFileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SmallFileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChatOwnerId")
                        .IsUnique();

                    b.ToTable("ChatsPhotos");
                });

            modelBuilder.Entity("SharedLib.ChatTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<long>("ChatTelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool?>("IsForum")
                        .HasColumnType("boolean");

                    b.Property<int>("LastMessageId")
                        .HasColumnType("integer");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedFirstNameUpper")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedLastNameUpper")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedTitleUpper")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUsernameUpper")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChatTelegramId")
                        .IsUnique();

                    b.HasIndex("FirstName");

                    b.HasIndex("IsForum");

                    b.HasIndex("LastName");

                    b.HasIndex("LastUpdateUtc");

                    b.HasIndex("NormalizedFirstNameUpper");

                    b.HasIndex("NormalizedLastNameUpper");

                    b.HasIndex("NormalizedTitleUpper");

                    b.HasIndex("NormalizedUsernameUpper");

                    b.HasIndex("Title");

                    b.HasIndex("Type");

                    b.HasIndex("Username");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("SharedLib.ContactTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("Vcard")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("SharedLib.DocumentTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<long?>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<string>("MimeType")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("SharedLib.DocumentThumbnailTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DocumentOwnerId")
                        .HasColumnType("integer");

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DocumentOwnerId")
                        .IsUnique();

                    b.HasIndex("MessageId");

                    b.ToTable("DocumentsThumbnails");
                });

            modelBuilder.Entity("SharedLib.ErrorSendingMessageTelegramBotModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("ErrorCode")
                        .HasColumnType("integer");

                    b.Property<string>("ExceptionTypeName")
                        .HasColumnType("text");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEditing")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("IsDisabled");

                    b.ToTable("ErrorsSendingTextMessageTelegramBot");
                });

            modelBuilder.Entity("SharedLib.JoinUserChatModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ChatId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("UserId", "ChatId")
                        .IsUnique();

                    b.ToTable("JoinsUsersToChats");
                });

            modelBuilder.Entity("SharedLib.MessageTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AudioId")
                        .HasColumnType("integer");

                    b.Property<string>("AuthorSignature")
                        .HasColumnType("text");

                    b.Property<string>("Caption")
                        .HasColumnType("text");

                    b.Property<int>("ChatId")
                        .HasColumnType("integer");

                    b.Property<int?>("ContactId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("DocumentId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("EditDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ForwardDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("ForwardFromChatId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ForwardFromId")
                        .HasColumnType("bigint");

                    b.Property<int?>("ForwardFromMessageId")
                        .HasColumnType("integer");

                    b.Property<string>("ForwardSenderName")
                        .HasColumnType("text");

                    b.Property<string>("ForwardSignature")
                        .HasColumnType("text");

                    b.Property<int?>("FromId")
                        .HasColumnType("integer");

                    b.Property<bool?>("IsAutomaticForward")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsTopicMessage")
                        .HasColumnType("boolean");

                    b.Property<string>("MediaGroupId")
                        .HasColumnType("text");

                    b.Property<int>("MessageTelegramId")
                        .HasColumnType("integer");

                    b.Property<int?>("MessageThreadId")
                        .HasColumnType("integer");

                    b.Property<string>("NormalizedCaptionUpper")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedTextUpper")
                        .HasColumnType("text");

                    b.Property<int?>("ReplyToMessageId")
                        .HasColumnType("integer");

                    b.Property<int?>("SenderChatId")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<long?>("ViaBotId")
                        .HasColumnType("bigint");

                    b.Property<int?>("VideoId")
                        .HasColumnType("integer");

                    b.Property<int?>("VoiceId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("FromId");

                    b.HasIndex("MessageTelegramId", "ChatId", "FromId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SharedLib.PhotoMessageTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.ToTable("PhotosMessages");
                });

            modelBuilder.Entity("SharedLib.UserTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool?>("AddedToAttachmentMenu")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsBot")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsPremium")
                        .HasColumnType("boolean");

                    b.Property<string>("LanguageCode")
                        .HasColumnType("text");

                    b.Property<int>("LastMessageId")
                        .HasColumnType("integer");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedFirstNameUpper")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedLastNameUpper")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUsernameUpper")
                        .HasColumnType("text");

                    b.Property<long>("UserTelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FirstName");

                    b.HasIndex("IsBot");

                    b.HasIndex("LastName");

                    b.HasIndex("UserTelegramId")
                        .IsUnique();

                    b.HasIndex("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SharedLib.VideoTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<long?>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<string>("MimeType")
                        .HasColumnType("text");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("SharedLib.VideoThumbnailTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<int>("VideoOwnerId")
                        .HasColumnType("integer");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("VideoOwnerId")
                        .IsUnique();

                    b.ToTable("VideosThumbnails");
                });

            modelBuilder.Entity("SharedLib.VoiceTelegramModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<string>("MimeType")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.ToTable("Voices");
                });

            modelBuilder.Entity("SharedLib.AudioTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.MessageTelegramModelDB", "Message")
                        .WithOne("Audio")
                        .HasForeignKey("SharedLib.AudioTelegramModelDB", "MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SharedLib.AudioThumbnailTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.AudioTelegramModelDB", "AudioOwner")
                        .WithOne("AudioThumbnail")
                        .HasForeignKey("SharedLib.AudioThumbnailTelegramModelDB", "AudioOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.MessageTelegramModelDB", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AudioOwner");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SharedLib.ChatPhotoTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.ChatTelegramModelDB", "ChatOwner")
                        .WithOne("ChatPhoto")
                        .HasForeignKey("SharedLib.ChatPhotoTelegramModelDB", "ChatOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChatOwner");
                });

            modelBuilder.Entity("SharedLib.ContactTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.MessageTelegramModelDB", "Message")
                        .WithOne("Contact")
                        .HasForeignKey("SharedLib.ContactTelegramModelDB", "MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SharedLib.DocumentTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.MessageTelegramModelDB", "Message")
                        .WithOne("Document")
                        .HasForeignKey("SharedLib.DocumentTelegramModelDB", "MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SharedLib.DocumentThumbnailTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.DocumentTelegramModelDB", "DocumentOwner")
                        .WithOne("ThumbnailDocument")
                        .HasForeignKey("SharedLib.DocumentThumbnailTelegramModelDB", "DocumentOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.MessageTelegramModelDB", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DocumentOwner");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SharedLib.JoinUserChatModelDB", b =>
                {
                    b.HasOne("SharedLib.ChatTelegramModelDB", "Chat")
                        .WithMany("UsersJoins")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.UserTelegramModelDB", "User")
                        .WithMany("ChatsJoins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SharedLib.MessageTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.ChatTelegramModelDB", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.UserTelegramModelDB", "From")
                        .WithMany("Messages")
                        .HasForeignKey("FromId");

                    b.Navigation("Chat");

                    b.Navigation("From");
                });

            modelBuilder.Entity("SharedLib.PhotoMessageTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.MessageTelegramModelDB", "Message")
                        .WithMany("Photo")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SharedLib.VideoTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.MessageTelegramModelDB", "Message")
                        .WithOne("Video")
                        .HasForeignKey("SharedLib.VideoTelegramModelDB", "MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SharedLib.VideoThumbnailTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.MessageTelegramModelDB", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.VideoTelegramModelDB", "VideoOwner")
                        .WithOne("ThumbnailVideo")
                        .HasForeignKey("SharedLib.VideoThumbnailTelegramModelDB", "VideoOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");

                    b.Navigation("VideoOwner");
                });

            modelBuilder.Entity("SharedLib.VoiceTelegramModelDB", b =>
                {
                    b.HasOne("SharedLib.MessageTelegramModelDB", "Message")
                        .WithOne("Voice")
                        .HasForeignKey("SharedLib.VoiceTelegramModelDB", "MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SharedLib.AudioTelegramModelDB", b =>
                {
                    b.Navigation("AudioThumbnail");
                });

            modelBuilder.Entity("SharedLib.ChatTelegramModelDB", b =>
                {
                    b.Navigation("ChatPhoto");

                    b.Navigation("Messages");

                    b.Navigation("UsersJoins");
                });

            modelBuilder.Entity("SharedLib.DocumentTelegramModelDB", b =>
                {
                    b.Navigation("ThumbnailDocument");
                });

            modelBuilder.Entity("SharedLib.MessageTelegramModelDB", b =>
                {
                    b.Navigation("Audio");

                    b.Navigation("Contact");

                    b.Navigation("Document");

                    b.Navigation("Photo");

                    b.Navigation("Video");

                    b.Navigation("Voice");
                });

            modelBuilder.Entity("SharedLib.UserTelegramModelDB", b =>
                {
                    b.Navigation("ChatsJoins");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("SharedLib.VideoTelegramModelDB", b =>
                {
                    b.Navigation("ThumbnailVideo");
                });
#pragma warning restore 612, 618
        }
    }
}
