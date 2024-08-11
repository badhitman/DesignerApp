////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using MudBlazor;
using SharedLib;

namespace BlazorLib;

/// <summary>
/// Tree Item Data Rubric
/// </summary>
public class TreeItemDataRubricModel : TreeItemData<RubricIssueHelpdeskLowModel?>
{
    /// <summary>
    /// Tree Item Data Rubric
    /// </summary>
    public TreeItemDataRubricModel(RubricIssueHelpdeskLowModel entry, string icon) : base(entry)
    {
        Text = entry.Name;
        Icon = icon;
        Expandable = entry.Id > 0;
    }

    /// <summary>
    /// Признак того является ли элемент крайним в перечне
    /// </summary>
    /// <remarks>
    /// Для определения доступности кнопок перемещения элементов вверх/вниз
    /// </remarks>
    public VerticalDirectionsEnum? MostHavePosition { get; set; }

    /// <inheritdoc/>
    public static bool operator ==(TreeItemDataRubricModel? e1, TreeItemDataRubricModel? e2)
        => (e1 is null && e2 is null) || e1?.Value == e2?.Value;

    /// <inheritdoc/>
    public static bool operator !=(TreeItemDataRubricModel? e1, TreeItemDataRubricModel? e2)
        => e1?.Value != e2?.Value;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is TreeItemDataRubricModel _e)
            return Value == _e.Value;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => Value!.GetHashCode();
}