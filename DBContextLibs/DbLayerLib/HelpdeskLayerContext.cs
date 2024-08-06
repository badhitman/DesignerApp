////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class HelpdeskLayerContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
#if DEBUG
        options.LogTo(Console.WriteLine);
#endif
    }

    /// <summary>
    /// IssuesThemes
    /// </summary>
    public DbSet<IssueThemeModelDB> IssuesThemes { get; set; }

    /// <summary>
    /// IssuesThemes
    /// </summary>
    public DbSet<IssueModelDB> Issues { get; set; }

    /// <summary>
    /// Подписчики
    /// </summary>
    public DbSet<SubscriberIssueModelDB> SubscribersOfIssues { get; set; }

    /// <summary>
    /// IssueReadMarkers
    /// </summary>
    public DbSet<IssueReadMarkerModelDB> IssueReadMarkers { get; set; }

    /// <summary>
    /// IssuesMessages
    /// </summary>
    public DbSet<IssueMessageModelDB> IssuesMessages { get; set; }

    /// <summary>
    /// MarkAsResponses
    /// </summary>
    public DbSet<MarkAsResponseModelDB> MarkAsResponses { get; set; }
}