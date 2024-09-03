////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <inheritdoc/>
public partial class RubricsManageComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <summary>
    /// Имя контекста
    /// </summary>
    [Parameter]
    public string? ContextName { get; set; }


    List<TreeItemDataRubricModel> InitialTreeItems { get; set; } = [];

    static List<TreeItemData<RubricIssueHelpdeskLowModel?>> ConvertRubrics(IEnumerable<RubricIssueHelpdeskLowModel> rubrics)
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

                return new TreeItemDataRubricModel(x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : Icons.Material.Filled.CropFree){ MoveRowState = mhp };
            })];
    }

    void ItemUpdAction(RubricIssueHelpdeskLowModel sender)
    {
        TreeItemDataRubricModel findNode = FindNode(sender.Id, InitialTreeItems) ?? throw new Exception();
        findNode.Text = sender.Name;
        findNode.Value?.Update(sender);
    }

    async void ReloadNodeAction(int parent_id)
    {
        List<RubricIssueHelpdeskLowModel> rubrics = await RequestRubrics(parent_id);
        if (parent_id > 0)
        {
            TreeItemDataRubricModel findNode = FindNode(parent_id, InitialTreeItems) ?? throw new Exception();
            findNode.Children = ConvertRubrics(rubrics);
        }
        else
        {
            InitialTreeItems = [.. ConvertRubrics(rubrics).Cast<TreeItemDataRubricModel>()];
        }

        StateHasChanged();
    }

    static TreeItemDataRubricModel? FindNode(int parent_id, IEnumerable<TreeItemDataRubricModel> treeItems)
    {
        TreeItemDataRubricModel? res = treeItems.FirstOrDefault(x => x.Value?.Id == parent_id);
        if (res is not null)
            return res;

        TreeItemDataRubricModel? FindChildNode(List<TreeItemData<RubricIssueHelpdeskLowModel?>> children)
        {
            TreeItemData<RubricIssueHelpdeskLowModel?>? res_child = children.FirstOrDefault(x => x.Value?.Id == parent_id);
            if (res_child is not null)
                return (TreeItemDataRubricModel?)res_child;

            foreach (TreeItemData<RubricIssueHelpdeskLowModel?> c in children)
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
        List<RubricIssueHelpdeskLowModel> rubrics = await RequestRubrics();
        InitialTreeItems = [.. ConvertRubrics(rubrics).Cast<TreeItemDataRubricModel>()];
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TreeItemData<RubricIssueHelpdeskLowModel?>>> LoadServerData(RubricIssueHelpdeskLowModel? parentValue)
    {
        ArgumentNullException.ThrowIfNull(parentValue);

        List<RubricIssueHelpdeskLowModel> rubrics = await RequestRubrics(parentValue.Id);
        TreeItemDataRubricModel findNode = FindNode(parentValue.Id, InitialTreeItems) ?? throw new Exception();

        findNode.Children = ConvertRubrics(rubrics);
        return findNode.Children;
    }

    async Task<List<RubricIssueHelpdeskLowModel>> RequestRubrics(int? parent_id = null)
    {
        IsBusyProgress = true;
        TResponseModel<List<RubricIssueHelpdeskLowModel>?> rest = await HelpdeskRepo.RubricsList(new() { Request = parent_id ?? 0, ContextName = ContextName });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Response is null)
            throw new Exception();

        rest.Response = [.. rest.Response.OrderBy(x => x.SortIndex)];

        rest.Response.Add(new RubricIssueHelpdeskLowModel() { Name = "", SortIndex = uint.MaxValue, ParentRubricId = parent_id });
        return rest.Response;
    }
}