////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SelectIssuesRequestModel
/// </summary>
public class SelectIssuesRequestModel : SelectRequestBaseModel
{
    /// <summary>
    /// Автор, Исполнитель, Подписчик или Main (= Исполнитель||Подписчик)
    /// </summary>
    public UsersAreasHelpdeskEnum? UserArea { get; set; }

    /// <summary>
    /// Journal mode: All, ActualOnly, ArchiveOnly
    /// </summary>
    public required HelpdeskJournalModesEnum JournalMode { get; set; }

    /// <summary>
    /// Загрузить данные по подписчикам
    /// </summary>
    public bool IncludeSubscribers { get; set; }
}