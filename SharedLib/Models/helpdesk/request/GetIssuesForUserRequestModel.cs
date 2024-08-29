////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// GetIssuesForUsersRequestModel
/// </summary>
public class GetIssuesForUserRequestModel
{
    /// <summary>
    /// IdentityUserId
    /// </summary>
    [Required]
    public required string[] IdentityUsersIds { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Строка поиска
    /// </summary>
    public string? SearchQuery { get; set; }

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