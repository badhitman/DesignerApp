////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Entry Document Type
/// </summary>
public class EntryDocumentTypeModel(DocumentFitModel docObj, string documentsDirectoryPath, string blazorDirectoryPath)
    : EntryTypeModel(docObj.SystemName, documentsDirectoryPath)
{
    /// <summary>
    /// Document
    /// </summary>
    public DocumentFitModel Document { get; set; } = docObj;

    /// <summary>
    /// Blazor directory path
    /// </summary>
    public string BlazorDirectoryPath { get; set; } = blazorDirectoryPath;

    /// <summary>
    /// Полный путь/имя файла типа данных (Blazor component)
    /// </summary>
    /// <param name="postfix_type_name">Постфикс имени компонента Blazor. Например: <c>Page</c> (по умолчанию), для объявления страниц или <c>Component</c> для остальных <c>Blazor Components</c></param>
    /// <returns>Путь к элементу в архиве</returns>
    public string BlazorFullEntryName(string postfix_type_name = "Page") => string.IsNullOrWhiteSpace(PrefixPath)
        ? $"{Path.Combine(BlazorDirectoryPath, $"{TypeName}")}{postfix_type_name}.razor"
        : $"{Path.Combine(BlazorDirectoryPath, PrefixPath, $"{TypeName}")}{postfix_type_name}.razor";
}