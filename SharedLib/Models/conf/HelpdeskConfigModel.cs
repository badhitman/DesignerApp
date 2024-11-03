////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////


namespace SharedLib;

/// <summary>
/// Helpdesk Config
/// </summary>
public class HelpdeskConfigModel : WebConfigModel
{
    /// <summary>
    /// Длительность Cache для сегментов консоли (по статусу)
    /// </summary>
    public int ConsoleSegmentCacheLifetimeSeconds { get; set; } = 60 * 60 * 24 * 7;//неделя
}
