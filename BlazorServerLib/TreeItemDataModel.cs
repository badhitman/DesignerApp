using MudBlazor;
using SharedLib;

namespace BlazorWebLib;

/// <inheritdoc/>
public class TreeItemDataModel : TreeItemData<EntryTagModel>
{
    /// <summary>
    /// Всплывающая подсказка
    /// </summary>
    public string? Tooltip { get; set; }

    /// <inheritdoc/>
    public TreeItemDataModel(EntryTagModel entry, string icon) : base(entry)
    {
        Text = entry.Name;
        Icon = icon;        
    }
}
