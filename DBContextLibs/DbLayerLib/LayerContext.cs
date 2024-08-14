////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbLayerLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class LayerContext : DbContext
{
    /// <summary>
    /// Промежуточный/общий слой контекста базы данных
    /// </summary>
    public LayerContext(DbContextOptions options)
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
    /// Telegram пользователи
    /// </summary>
    public DbSet<TelegramUserModelDb> TelegramUsers { get; set; }

    /// <summary>
    /// Действия, связанные с подключения Telegram аккаунта к учётной записи сайта
    /// </summary>
    public DbSet<TelegramJoinAccountModelDb> TelegramJoinActions { get; set; }
}