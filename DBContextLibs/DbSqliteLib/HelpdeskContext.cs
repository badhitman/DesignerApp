////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class HelpdeskContext : HelpdeskLayerContext
{
    /// <summary>
    /// Контекст доступа к SQLite БД
    /// </summary>
    public HelpdeskContext(DbContextOptions<HelpdeskContext> options) : base(options)
    {
        Database.Migrate();
    }
}