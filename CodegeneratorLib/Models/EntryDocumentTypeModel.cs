////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SharedLib;

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
    public string BlazorFormFullEntryName(string postfix_type_name = "Page") => string.IsNullOrWhiteSpace(PrefixPath)
        ? $"{Path.Combine(BlazorDirectoryPath, BlazorComponentName(postfix_type_name))}.razor"
        : $"{Path.Combine(BlazorDirectoryPath, PrefixPath, BlazorComponentName(postfix_type_name))}.razor";

    /// <summary>
    /// Blazor Component name
    /// </summary>
    public string BlazorComponentName(string postfix_type_name = "Page") => $"{TypeName}{postfix_type_name}";
}