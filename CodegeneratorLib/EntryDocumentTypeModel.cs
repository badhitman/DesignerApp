using SharedLib.Models;

namespace SharedLib;

/// <summary>
/// Entry Document Type
/// </summary>
public class EntryDocumentTypeModel(DocumentFitModel docObj, string basePath) : EntryTypeModel(docObj.SystemName, basePath)
{//
    /// <summary>
    /// Название документа
    /// </summary>
    public string Name { get; set; } = docObj.Name;

    /// <summary>
    /// Описание документа
    /// </summary>
    public string? Description { get; set; } = docObj.Description;
}