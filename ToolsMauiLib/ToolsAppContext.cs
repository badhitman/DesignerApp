////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class ToolsAppContext(DbContextOptions<ToolsAppContext> options) : ToolsAppLayerContext(options)
{
    /// <summary>
    /// FileName
    /// </summary>
    private static readonly string _ctxName = nameof(ToolsAppContext);
    protected override string CtxName => _ctxName;

    /// <summary>
    /// db Path
    /// </summary>
    protected override string DbPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CtxName, $"{AppDomain.CurrentDomain.FriendlyName}.db3");

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        base.OnConfiguring(options);

        options
            .UseSqlite($"Filename={DbPath}");
    }
}