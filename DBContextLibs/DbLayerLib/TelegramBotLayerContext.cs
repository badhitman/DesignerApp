////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class TelegramBotLayerContext : DbContext
{
    /// <summary>
    /// Промежуточный/общий слой контекста базы данных
    /// </summary>
    public TelegramBotLayerContext(DbContextOptions options)
        : base(options)
    {
#if DEBUG
        Database.EnsureCreated();
#else
        Database.Migrate();
#endif
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MessageTelegramModelDB>()
            .HasOne(a => a.Audio)
            .WithOne(a => a.Message)
            .HasForeignKey<AudioTelegramModelDB>(c => c.MessageId);

        modelBuilder.Entity<MessageTelegramModelDB>()
            .HasOne(a => a.Document)
            .WithOne(a => a.Message)
            .HasForeignKey<DocumentTelegramModelDB>(c => c.MessageId);

        modelBuilder.Entity<MessageTelegramModelDB>()
            .HasOne(a => a.Video)
            .WithOne(a => a.Message)
            .HasForeignKey<VideoTelegramModelDB>(c => c.MessageId);

        modelBuilder.Entity<MessageTelegramModelDB>()
            .HasOne(a => a.Voice)
            .WithOne(a => a.Message)
            .HasForeignKey<VoiceTelegramModelDB>(c => c.MessageId);

        modelBuilder.Entity<MessageTelegramModelDB>()
            .HasOne(a => a.Contact)
            .WithOne(a => a.Message)
            .HasForeignKey<ContactTelegramModelDB>(c => c.MessageId);
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
#if DEBUG
        options.LogTo(Console.WriteLine);
#endif
    }

    /// <summary>
    /// JoinsUsersToChats
    /// </summary>
    public DbSet<JoinUserChatModelDB> JoinsUsersToChats { get; set; }

    /// <summary>
    /// Chats
    /// </summary>
    public DbSet<ChatTelegramModelDB> Chats { get; set; }

    /// <summary>
    /// Chats Photos
    /// </summary>
    public DbSet<ChatPhotoTelegramModelDB> ChatsPhotos { get; set; }


    /// <summary>
    /// Users
    /// </summary>
    public DbSet<UserTelegramModelDB> Users { get; set; }


    /// <summary>
    /// Messages
    /// </summary>
    public DbSet<MessageTelegramModelDB> Messages { get; set; }

    /// <summary>
    /// Photos Messages
    /// </summary>
    public DbSet<PhotoMessageTelegramModelDB> PhotosMessages { get; set; }


    /// <summary>
    /// Audios
    /// </summary>
    public DbSet<AudioTelegramModelDB> Audios { get; set; }

    /// <summary>
    /// AudiosThumbnails
    /// </summary>
    public DbSet<AudioThumbnailTelegramModelDB> AudiosThumbnails { get; set; }


    /// <summary>
    /// Documents
    /// </summary>
    public DbSet<DocumentTelegramModelDB> Documents { get; set; }

    /// <summary>
    /// DocumentsThumbnails
    /// </summary>
    public DbSet<DocumentThumbnailTelegramModelDB> DocumentsThumbnails { get; set; }


    /// <summary>
    /// Videos
    /// </summary>
    public DbSet<VideoTelegramModelDB> Videos { get; set; }

    /// <summary>
    /// VideosThumbnails
    /// </summary>
    public DbSet<VideoThumbnailTelegramModelDB> VideosThumbnails { get; set; }


    /// <summary>
    /// Voices
    /// </summary>
    public DbSet<VoiceTelegramModelDB> Voices { get; set; }


    /// <summary>
    /// Contacts
    /// </summary>
    public DbSet<ContactTelegramModelDB> Contacts { get; set; }

    /// <summary>
    /// Ошибки отправки сообщений TelegramBot
    /// </summary>
    public DbSet<ErrorSendingMessageTelegramBotModelDB> ErrorsSendingTextMessageTelegramBot { get; set; }
}