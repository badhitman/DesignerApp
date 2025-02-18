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
    static readonly string FileName = nameof(ToolsAppContext);

    /// <summary>
    /// db Path
    /// </summary>
    public static string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"{FileName}.db3");

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        base.OnConfiguring(options);
        
        options
            .UseSqlite($"Filename={dbPath}");
    }
}