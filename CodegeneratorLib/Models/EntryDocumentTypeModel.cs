////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Entry Document Type
/// </summary>
public class EntryDocumentTypeModel(DocumentFitModel docObj, string basePath) : EntryTypeModel(docObj.SystemName, basePath)
{
    /// <summary>
    /// Document
    /// </summary>
    public DocumentFitModel Document { get; set; } = docObj;
}