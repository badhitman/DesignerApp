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
//#if DEBUG
//        Database.EnsureCreated();
//#else
        Database.Migrate();
//#endif
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
        options.EnableSensitiveDataLogging(true);
        options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
#endif
    }

    /// <summary>
    /// JoinsUsersToChats
    /// </summary>
    public DbSet<JoinUserChatModelDB> JoinsUsersToChats { get; set; } = default!;

    /// <summary>
    /// Chats
    /// </summary>
    public DbSet<ChatTelegramModelDB> Chats { get; set; } = default!;

    /// <summary>
    /// Chats Photos
    /// </summary>
    public DbSet<ChatPhotoTelegramModelDB> ChatsPhotos { get; set; } = default!;


    /// <summary>
    /// Users
    /// </summary>
    public DbSet<UserTelegramModelDB> Users { get; set; } = default!;


    /// <summary>
    /// Messages
    /// </summary>
    public DbSet<MessageTelegramModelDB> Messages { get; set; } = default!;

    /// <summary>
    /// Photos Messages
    /// </summary>
    public DbSet<PhotoMessageTelegramModelDB> PhotosMessages { get; set; } = default!;


    /// <summary>
    /// Audios
    /// </summary>
    public DbSet<AudioTelegramModelDB> Audios { get; set; } = default!;

    /// <summary>
    /// AudiosThumbnails
    /// </summary>
    public DbSet<AudioThumbnailTelegramModelDB> AudiosThumbnails { get; set; } = default!;


    /// <summary>
    /// Documents
    /// </summary>
    public DbSet<DocumentTelegramModelDB> Documents { get; set; } = default!;

    /// <summary>
    /// DocumentsThumbnails
    /// </summary>
    public DbSet<DocumentThumbnailTelegramModelDB> DocumentsThumbnails { get; set; } = default!;


    /// <summary>
    /// Videos
    /// </summary>
    public DbSet<VideoTelegramModelDB> Videos { get; set; } = default!;

    /// <summary>
    /// VideosThumbnails
    /// </summary>
    public DbSet<VideoThumbnailTelegramModelDB> VideosThumbnails { get; set; } = default!;


    /// <summary>
    /// Voices
    /// </summary>
    public DbSet<VoiceTelegramModelDB> Voices { get; set; } = default!;


    /// <summary>
    /// Contacts
    /// </summary>
    public DbSet<ContactTelegramModelDB> Contacts { get; set; } = default!;

    /// <summary>
    /// Ошибки отправки сообщений TelegramBot
    /// </summary>
    public DbSet<ErrorSendingMessageTelegramBotModelDB> ErrorsSendingTextMessageTelegramBot { get; set; } = default!;
}