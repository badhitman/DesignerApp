////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <inheritdoc/>
public partial class ToolsAppLayerContext : DbContext
{
    /// <inheritdoc/>
    public ToolsAppLayerContext(DbContextOptions options)
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
        options.EnableSensitiveDataLogging(true);
        options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
#endif
    }

    /// <summary>
    /// Настройки подключения к api/rest
    /// </summary>
    public DbSet<ApiRestConfigModelDB> Configurations { get; set; }

    /// <summary>
    /// Папки синхронизации
    /// </summary>
    public DbSet<SyncDirectoryModelDB> SyncDirectories { get; set; }
}