////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Journal Universal
/// </summary>
public partial interface IJournalUniversalService
{
    /// <summary>
    /// Получить колонки документа по его имени
    /// </summary>
    public Task<TResponseModel<EntryAltModel[]?>> GetColumnsForJournal(string document_name, int? projectId);

    /// <summary>
    /// Получить порцию документов
    /// </summary>
    public Task<TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>>> SelectJournalPart(SelectJournalPartRequestModel req, int? projectId);
}