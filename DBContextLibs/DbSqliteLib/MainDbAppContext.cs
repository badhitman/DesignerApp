////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbLayerLib;
using Microsoft.EntityFrameworkCore;

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
#if DEBUG

#else
        Database.Migrate();
#endif

    }
}