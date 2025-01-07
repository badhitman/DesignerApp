////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <inheritdoc/>
public partial class RubricsManageComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <summary>
    /// Имя контекста
    /// </summary>
    [Parameter]
    public string? ContextName { get; set; }

    /// <summary>
    /// ValueChanged
    /// </summary>
    [Parameter]
    public TreeViewOptionsModel? SelectedValuesChanged { get; set; }

    /// <summary>
    /// Без вложенных узлов
    /// </summary>
    [Parameter]
    public bool SingleLevelMode { get; set; }


    List<TreeItemDataRubricModel> InitialTreeItems { get; set; } = [];
    void SelectedValuesChangeHandler(IReadOnlyCollection<UniversalBaseModel?> SelectedValues)
    {
        SelectedValuesChanged?.SelectedValuesChangedHandler(SelectedValues);
    }

    List<TreeItemData<UniversalBaseModel>> ConvertRubrics(IEnumerable<UniversalBaseModel> rubrics)
    {
        (uint min, uint max) = rubrics.Any(x => x.SortIndex != uint.MaxValue)
            ? (rubrics.Min(x => x.SortIndex), rubrics.Where(x => x.SortIndex != uint.MaxValue).Max(x => x.SortIndex))
            : (0, 0);

        return [.. rubrics.Select(x => {
                MoveRowStatesEnum mhp;
            if(x.SortIndex == min && x.SortIndex == max)
                mhp = MoveRowStatesEnum.Singleton;
            else if(x.SortIndex == min)
                mhp = MoveRowStatesEnum.Start;
            else if(x.SortIndex == max)
                mhp = MoveRowStatesEnum.End;
            else
                mhp = MoveRowStatesEnum.Between;

            TreeItemDataRubricModel _ri = new (x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : SelectedValuesChanged is null ? Icons.Material.Filled.CropFree : Icons.Custom.Uncategorized.Folder )
                {
                    MoveRowState = mhp,
                    Selected = SelectedValuesChanged?.SelectedNodes.Contains(x.Id) == true
                };

            if(SingleLevelMode)
                _ri.Expandable = false;

            return _ri;
        })];
    }

    void ItemUpdAction(UniversalBaseModel sender)
    {
        TreeItemDataRubricModel findNode = FindNode(sender.Id, InitialTreeItems) ?? throw new Exception();
        findNode.Text = sender.Name;
        findNode.Value?.Update(sender);
    }

    async void ReloadNodeAction(int parent_id)
    {
        List<UniversalBaseModel> rubrics = await RequestRubrics(parent_id);
        if (parent_id > 0)
        {
            TreeItemDataRubricModel findNode = FindNode(parent_id, InitialTreeItems) ?? throw new Exception();
            findNode.Children = ConvertRubrics(rubrics)!;
        }
        else
        {
            InitialTreeItems = [.. ConvertRubrics(rubrics).Select(x => new TreeItemDataRubricModel(x))]; //.Cast<TreeItemDataRubricModel>()];
        }
        await SetBusy(false);
    }

    static TreeItemDataRubricModel? FindNode(int parent_id, IEnumerable<TreeItemDataRubricModel> treeItems)
    {
        TreeItemDataRubricModel? res = treeItems.FirstOrDefault(x => x.Value?.Id == parent_id);
        if (res is not null)
            return res;

        TreeItemDataRubricModel? FindChildNode(List<TreeItemData<UniversalBaseModel?>> children)
        {
            TreeItemData<UniversalBaseModel?>? res_child = children.FirstOrDefault(x => x.Value?.Id == parent_id);
            if (res_child is not null)
                return (TreeItemDataRubricModel?)res_child;

            foreach (TreeItemData<UniversalBaseModel?> c in children)
            {
                if (c.Children is not null)
                {
                    res_child = FindChildNode(c.Children);
                    if (res_child is not null)
                        return (TreeItemDataRubricModel?)res_child;
                }
            }

            return null;
        }

        foreach (TreeItemDataRubricModel _tin in treeItems)
        {
            if (_tin.Children is not null)
            {
                res = FindChildNode(_tin.Children);
                if (res is not null)
                    return res;
            }
        }

        return null;
    }

    /// <inheritdoc/>
    protected override async void OnInitialized()
    {
        List<UniversalBaseModel> rubrics = await RequestRubrics();
        InitialTreeItems = [.. ConvertRubrics(rubrics).Select(x => new TreeItemDataRubricModel(x))];
        await SetBusy(false);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TreeItemData<UniversalBaseModel?>>> LoadServerData(UniversalBaseModel? parentValue)
    {
        ArgumentNullException.ThrowIfNull(parentValue);

        List<UniversalBaseModel> rubrics = await RequestRubrics(parentValue.Id);
        TreeItemDataRubricModel findNode = FindNode(parentValue.Id, InitialTreeItems) ?? throw new Exception();

        findNode.Children = ConvertRubrics(rubrics)!;
        //if ()
        //    findNode.Children.ForEach(r => { r.Expandable = false; });

        return findNode.Children;
    }

    async Task<List<UniversalBaseModel>> RequestRubrics(int? parent_id = null)
    {
        await SetBusy();
        List<UniversalBaseModel> rest = await HelpdeskRepo.RubricsList(new() { Request = parent_id ?? 0, ContextName = ContextName });

        rest = [.. rest.OrderBy(x => x.SortIndex)];

        if (SelectedValuesChanged is null)
            rest.Add(new UniversalBaseModel() { Name = "", SortIndex = uint.MaxValue, ParentId = parent_id });

        return rest;
    }
}