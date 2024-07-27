////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос порции строк журнала
/// </summary>
public class SelectJournalPartRequestModel : PaginationRequestModel
{
    /// <summary>
    /// Номер или имя документа
    /// </summary>
    public required string DocumentNameOrId { get; set; }

    /// <summary>
    /// Строка поиска
    /// </summary>
    public string? SearchString { get; set; }
}