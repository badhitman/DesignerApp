////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using DbLayerLib;
using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <summary>
/// Контекст доступа к SQLite БД
/// </summary>
public class MainDbAppContext : LayerContext
{
    /// <summary>
    /// Контекст доступа к SQLite БД
    /// </summary>
    public MainDbAppContext(DbContextOptions<MainDbAppContext> options) : base(options)
    {
        Database.Migrate();
    }
}