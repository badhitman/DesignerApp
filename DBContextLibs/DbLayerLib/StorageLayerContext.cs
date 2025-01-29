////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class StorageLayerContext : DbContext
{
    /// <summary>
    /// Промежуточный/общий слой контекста базы данных
    /// </summary>
    public StorageLayerContext(DbContextOptions options)
        : base(options)
    {
        //#if DEBUG
        //        Database.EnsureCreated();
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
    /// Параметры
    /// </summary>
    public DbSet<StorageCloudParameterModelDB> CloudProperties { get; set; } = default!;

    /// <summary>
    /// Файлы
    /// </summary>
    public DbSet<StorageFileModelDB> CloudFiles { get; set; } = default!;

    /// <summary>
    /// RulesFilesAccess
    /// </summary>
    public DbSet<AccessFileRuleModelDB> RulesFilesAccess { get; set; } = default!;

    /// <summary>
    /// Тэги
    /// </summary>
    public DbSet<TagModelDB> CloudTags { get; set; } = default!;
}

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
        //        Database.EnsureCreated();
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