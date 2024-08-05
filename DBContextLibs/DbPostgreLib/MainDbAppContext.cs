////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbLayerLib;
using Microsoft.EntityFrameworkCore;

namespace DbcLib;

/// <summary>
/// Контекст доступа к Postgres
/// </summary>
public class MainDbAppContext : LayerContext
{
    /// <summary>
    /// Контекст доступа к Postgres
    /// </summary>
    public MainDbAppContext(DbContextOptions<MainDbAppContext> options) : base(options)
    {
#if DEBUG
        Database.Migrate();
#else
        Database.EnsureCreated();
#endif
    }
}