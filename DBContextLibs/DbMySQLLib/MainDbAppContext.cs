////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using DbLayerLib;
using Microsoft.EntityFrameworkCore;

namespace DbcLib;

/// <summary>
/// Контекст доступа к MySQL БД
/// </summary>
public class MainDbAppContext : LayerContext
{
    /// <summary>
    /// Контекст доступа к MySQL БД
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