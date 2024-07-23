////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Entry type
/// </summary>
public class EntrySchemaTypeModel(FormFitModel form, TabFitModel tab, DocumentFitModel doc, string documentsDirectoryPath, string blazorDirectoryPath, string? prefixPath = null)
    : EntryTypeModel($"{form.SystemName}{tab.SystemName}{doc.SystemName}", documentsDirectoryPath, prefixPath)
{
    /// <summary>
    /// Blazor Directory Path
    /// </summary>
    public string BlazorDirectoryPath { get; set; } = blazorDirectoryPath;

    /// <summary>
    /// Таблица данных
    /// </summary>
    public bool IsTable => Form.IsTable;

    /// <summary>
    /// Документ
    /// </summary>
    public DocumentFitModel Document { get; set; } = doc;

    /// <summary>
    /// Вкладка/таб
    /// </summary>
    public TabFitModel Tab { get; set; } = tab;

    /// <summary>
    /// Форма
    /// </summary>
    public FormFitModel Form { get; set; } = form;

    /// <summary>
    /// Полный путь/имя файла формы (Blazor component)
    /// </summary>
    /// <param name="postfix_type_name">Постфикс имени компонента Blazor. Например: <c>Page</c> (по умолчанию), для объявления страниц или <c>Component</c> для остальных <c>Blazor Components</c></param>
    /// <returns>Путь к элементу в архиве</returns>
    public string BlazorFormFullEntryName(string? postfix_type_name = "Component")
        => $"{Path.Combine(BlazorDirectoryPath, "forms", $"{Form.SystemName}")}{postfix_type_name}.razor";
}