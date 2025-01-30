////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class NLogsLayerContext : DbContext
{
    /// <summary>
    /// Промежуточный/общий слой контекста базы данных
    /// </summary>
    public NLogsLayerContext(DbContextOptions options)
        : base(options)
    {
        //#if DEBUG
        //  Database.EnsureCreated();
        //#else
        Database.Migrate();
        //#endif
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
#if DEBUG
        options.LogTo(Console.WriteLine);
#endif
    }

    /// <summary>
    /// Логи
    /// </summary>
    public DbSet<NLogRecordModelDB> Logs { get; set; } = default!;    
}