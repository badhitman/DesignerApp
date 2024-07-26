////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Journal Universal
/// </summary>
public interface IJournalUniversalService
{
    /// <summary>
    /// Получить колонки документа по его имени
    /// </summary>
    public Task<TResponseModel<EntryAltModel[]?>> GetColumnsForJournal(string document_name, int? projectId);

    /// <summary>
    /// Найти документ по его имени (или номеру)
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB[]?>> FindDocumentScheme(string document_name, int? projectId, bool includeTabs);

    /// <summary>
    /// Получить порцию документов
    /// </summary>
    public Task<TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>[]?>> SelectJournalPart(TPaginationRequestModel<string> req, int? projectId);
}