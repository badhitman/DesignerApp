////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Manufacture;

/// <summary>
/// EnumerationsManufacture
/// </summary>
public partial class EnumerationsManufactureComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ManufactureComponent ManufactureParentView { get; set; }


    const string icon_directory = Icons.Custom.Uncategorized.Folder;
    const string icon_element = Icons.Material.Filled.Label;

    /// <inheritdoc/>
    public List<TreeItemData<EntryTagModel>> TreeItems { get; private set; } = [];
    MudTreeView<EntryTagModel>? TreeView_ref;

    /// <summary>
    /// Перезагрузить дерево элементов
    /// </summary>
    public void ReloadTree()
    {
        ArgumentNullException.ThrowIfNull(ManufactureParentView.CurrentProject.Directories);
        TreeItems.Clear();
        ManufactureParentView
        .CurrentProject
        .Directories.ForEach(dir =>
        {
            TreeItemDataModel ElementOfDirectoryToTreeItem(ElementOfDirectoryConstructorModelDB el)
            {
                EntryTagModel _et = new()
                {
                    Id = el.Id,
                    Name = el.Name,
                    Tag = $"{ManufactureComponent.DirectoryTypeName}#{dir.Id} {ManufactureComponent.ElementOfDirectoryConstructorTypeName}",
                };

                TreeItemDataModel _ti = new(_et, icon_element)
                {
                    SystemName = ManufactureParentView.ParentFormsPage.SystemNamesManufacture.GetSystemName(_et.Id, _et.Tag),
                };

                return _ti;
            }

            EntryTagModel _et = EntryTagModel.Build(dir.Id, dir.Name, ManufactureComponent.DirectoryTypeName);
            TreeItemDataModel _ti = new(_et, icon_directory)
            {
                SystemName = ManufactureParentView.ParentFormsPage.SystemNamesManufacture.GetSystemName(dir.Id, ManufactureComponent.DirectoryTypeName),
                Children = dir.Elements is null ? null : [.. dir.Elements.Select(ElementOfDirectoryToTreeItem)],
                ErrorMessage = dir.Elements!.Count == 0 ? $"Список/справочник '{dir.Name}' не имеет элементов перечисления" : null,
            };

            TreeItems.Add(_ti);
        });
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ReloadTree();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (TreeView_ref is not null && firstRender)
            await TreeView_ref.ExpandAllAsync();
    }
}