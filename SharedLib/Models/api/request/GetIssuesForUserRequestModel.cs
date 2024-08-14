////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// GetIssuesForUserRequestModel
/// </summary>
public class GetIssuesForUserRequestModel
{
    /// <summary>
    /// IdentityUserId
    /// </summary>
    [Required] 
    public required string IdentityUserId { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Строка поиска
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// Автор, Исполнитель, Подписчик или Исполнитель||Подписчик
    /// </summary>
    public required UsersAreasHelpdeskEnum UserArea {  get; set; }

    /// <summary>
    /// JournalMode
    /// </summary>
    public required HelpdeskJournalModesEnum JournalMode { get; set; }
}