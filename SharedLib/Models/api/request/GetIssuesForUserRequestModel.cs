////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// GetIssuesForUserRequestModel
/// </summary>
public class GetIssuesForUserRequestModel : TPaginationRequestModel<UserCrossIdsModel>
{
    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Строка поиска
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// Автор, Исполнитель, Подписчик
    /// </summary>
    public UsersAreasHelpdeskEnum UserArea {  get; set; }

    /// <summary>
    /// JournalMode
    /// </summary>
    public required HelpdeskJournalModesEnum JournalMode { get; set; }
}