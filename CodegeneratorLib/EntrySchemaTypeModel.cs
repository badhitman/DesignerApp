using SharedLib.Models;

namespace SharedLib;

/// <summary>
/// Entry type
/// </summary>
public class EntrySchemaTypeModel(FormFitModel form, TabFitModel tab, DocumentFitModel doc, string basePath, string? prefixPath = null)
    : EntryTypeModel($"{form.SystemName}{tab.SystemName}{doc.SystemName}", basePath, prefixPath)
{
    /// <summary>
    /// Таблица данных
    /// </summary>
    public bool IsTable => Form.IsTable;

    /// <summary>
    /// Документ
    /// </summary>
    public DocumentFitModel Document = doc;

    /// <summary>
    /// Вкладка/таб
    /// </summary>
    public TabFitModel Tab = tab;

    /// <summary>
    /// Форма
    /// </summary>
    public FormFitModel Form = form;

    /// <summary>
    /// Полный путь/имя файла типа данных (Blazor component)
    /// </summary>
    /// <param name="postfix_type_name">Постфикс имени компонента Blazor. Например: <c>Page</c> (по умолчанию), для объявления страниц или <c>Component</c> для остальных <c>Blazor Components</c></param>
    /// <returns>Путь к элементу в архиве</returns>
    public string BlazorFullEntryName(string? postfix_type_name = "Page") => string.IsNullOrWhiteSpace(PrefixPath)
        ? $"{Path.Combine(BasePath, $"{TypeName}")}{postfix_type_name}.razor"
        : $"{Path.Combine(BasePath, PrefixPath, $"{TypeName}")}{postfix_type_name}.razor";
}