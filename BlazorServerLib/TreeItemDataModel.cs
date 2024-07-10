using MudBlazor;
using SharedLib;

namespace BlazorWebLib;

/// <inheritdoc/>
public class TreeItemDataModel : TreeItemData<EntryTagModel>
{
    /// <summary>
    /// Системное имя объекта
    /// </summary>
    public string? SystemName { get; set; }

    /// <summary>
    /// Квалификация сущности
    /// </summary>
    public string? Qualification { get; set; }

    /// <summary>
    /// Признак того что элемент не может быть выгружен в генератор кода.
    /// </summary>
    /// <remarks>
    /// Например поле типа [генератор] или пустое перечисление (без элементов)
    /// </remarks>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Всплывающая подсказка
    /// </summary>
    public string? Tooltip { get; set; }

    /// <summary>
    /// Сообщение об ошибке валидации.
    /// </summary>
    /// <remarks>
    /// При построении структуры/дерева выгрузки производится валидация узлов
    /// </remarks>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Информация
    /// </summary>
    public string? Information { get; set; }

    /// <inheritdoc/>
    public TreeItemDataModel(EntryTagModel entry, string icon) : base(entry)
    {
        Text = entry.Name;
        Icon = icon;
    }
}
