////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <inheritdoc/>
public abstract partial class ToolsAppLayerContext : DbContext
{
    /// <summary>
    /// FileName
    /// </summary>
    private static readonly string _ctxName = nameof(ToolsAppContext);

    /// <summary>
    /// db Path
    /// </summary>
    public static readonly string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _ctxName, $"{AppDomain.CurrentDomain.FriendlyName}.db3");


    /// <inheritdoc/>
    public ToolsAppLayerContext(DbContextOptions options)
        : base(options)
    {
        FileInfo _fi = new(DbPath);

        if (_fi.Directory?.Exists != true)
            Directory.CreateDirectory(Path.GetDirectoryName(DbPath)!);

        if (!_fi.Exists)
            Database.EnsureCreated();
        else
            Database.Migrate();
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


    /// <summary>
    /// Команды
    /// </summary>
    public DbSet<ExeCommandModelDB> ExeCommands { get; set; }
}