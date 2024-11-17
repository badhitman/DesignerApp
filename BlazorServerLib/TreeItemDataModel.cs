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

    /// <inheritdoc/>
    public TreeItemDataModel(TreeItemData<EntryTagModel> x)
    {
        TreeItemDataModel _sender = (TreeItemDataModel)x;
        Value = _sender.Value;
        Children = _sender.Children;

        SystemName = _sender.SystemName;
        Qualification = _sender.Qualification;
        Tooltip = _sender.Tooltip;
        ErrorMessage = _sender.ErrorMessage;
        Information = _sender.Information;
        Visible = _sender.Visible;
        IsDisabled = _sender.IsDisabled;
        Expandable = _sender.Expandable;
        Expanded = _sender.Expanded;
        Text = _sender.Text;
        Icon = _sender.Icon;
        SystemName = _sender.SystemName;
        Selected = _sender.Selected;
    }
}
