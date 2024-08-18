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
        Database.Migrate();
#else
        Database.EnsureCreated();
#endif
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
#if DEBUG
        options.LogTo(Console.WriteLine);
#endif
    }

    /// <summary>
    /// Chats
    /// </summary>
    public DbSet<ChatTelegramModelDB> Chats { get; set; }

    /// <summary>
    /// Users
    /// </summary>
    public DbSet<UserTelegramModelDB> Users { get; set; }

    /// <summary>
    /// Messages
    /// </summary>
    public DbSet<MessageTelegramModelDB> Messages { get; set; }

    /// <summary>
    /// Audios
    /// </summary>
    public DbSet<AudioTelegramModelDB> Audios { get; set; }

    /// <summary>
    /// Documents
    /// </summary>
    public DbSet<DocumentTelegramModelDB> Documents { get; set; }

    /// <summary>
    /// Videos
    /// </summary>
    public DbSet<VideoTelegramModelDB> Videos { get; set; }

    /// <summary>
    /// PhotoSizes
    /// </summary>
    public DbSet<PhotoSizeTelegramModelDB> PhotoSizes { get; set; }

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
    public DbSet<ErrorSendingTextMessageTelegramBotModelDB> ErrorsSendingTextMessageTelegramBot { get; set; }
}