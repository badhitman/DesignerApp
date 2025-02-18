////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class ToolsAppContext(DbContextOptions<ToolsAppContext> options) : ToolsAppLayerContext(options)
{
    /// <summary>
    /// FileName
    /// </summary>
    static readonly string ctxName = nameof(ToolsAppContext);

    /// <summary>
    /// db Path
    /// </summary>
    public static string DbPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ctxName, $"{AppDomain.CurrentDomain.FriendlyName}.db3");

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        base.OnConfiguring(options);
        
        options
            .UseSqlite($"Filename={DbPath}");
    }
}