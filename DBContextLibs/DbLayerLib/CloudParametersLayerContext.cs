////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class CloudParametersLayerContext : DbContext
{
    /// <summary>
    /// Промежуточный/общий слой контекста базы данных
    /// </summary>
    public CloudParametersLayerContext(DbContextOptions options)
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
    public DbSet<StorageCloudParameterModelDB> CloudProperties { get; set; }

    /// <summary>
    /// Файлы
    /// </summary>
    public DbSet<StorageFileModelDB> CloudFiles { get; set; }

    /// <summary>
    /// Тэги
    /// </summary>
    public DbSet<TagModelDB> CloudTags { get; set; }
}