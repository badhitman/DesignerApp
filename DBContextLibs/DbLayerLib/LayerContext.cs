////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbLayerLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class LayerContext(DbContextOptions options) : DbContext(options)
{
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
    public DbSet<TelegramJoinAccountModelDB> TelegramJoinActions { get; set; }
}