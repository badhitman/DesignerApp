namespace SharedLib;

public partial interface IJournalUniversalService
{
    /// <summary>
    /// Получить все свои документы:
    /// </summary>
    /// <remarks>
    /// имя документа и идентификатор проекта, которому принадлежит этот документ (а так же - имя проекта в Tag)
    /// </remarks>
    public Task<EntryAltTagModel[]> GetMyDocumentsSchemas();

    /// <summary>
    /// Найти схемы документов по имени (или номеру)
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB[]?>> FindDocumentSchemes(string document_name, int? projectId);
}