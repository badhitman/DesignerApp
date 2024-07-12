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
}