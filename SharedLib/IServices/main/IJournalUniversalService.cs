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
    /// Получить свои документы: имя документа и идентификатор проекта, которому принадлежит этот документ
    /// </summary>
    public Task<EntryAltTagModel[]> GetMyDocumentsSchemas();

    /// <summary>
    /// Получить колонки документа по его имени
    /// </summary>
    public Task<TResponseModel<EntryAltModel[]?>> GetColumnsForJournal(string document_name, int? projectId);

    /// <summary>
    /// Найти схемы документов по имени (или номеру)
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB[]?>> FindDocumentSchemes(string document_name, int? projectId);

    /// <summary>
    /// Получить порцию документов
    /// </summary>
    public Task<TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>>> SelectJournalPart(SelectJournalPartRequestModel req, int? projectId);
}