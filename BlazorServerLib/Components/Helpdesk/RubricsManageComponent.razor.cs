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


    List<TreeItemDataRubricModel> InitialTreeItems { get; set; } = [];

    async void ReloadNodeAction(int parent_id)
    {
        IsBusyProgress = true;
        List<RubricIssueHelpdeskLowModel> rubrics = await RequestRubrics(parent_id);

        (uint min, uint max) = rubrics.Count(x => x.SortIndex != uint.MaxValue) != 0
            ? (rubrics.Min(x => x.SortIndex), rubrics.Where(x => x.SortIndex != uint.MaxValue).Max(x => x.SortIndex))
            : (0, 0);

        IsBusyProgress = false;
        if (parent_id > 0)
        {
            TreeItemDataRubricModel findNode = FindNode(parent_id, InitialTreeItems) ?? throw new Exception();
            findNode.Children = [.. rubrics.Select(x => {
                VerticalDirectionsEnum? mhp = null;

                if(x.SortIndex == min)
                    mhp = VerticalDirectionsEnum.Up;
                else if(x.SortIndex == max)
                    mhp = VerticalDirectionsEnum.Down;

                return new TreeItemDataRubricModel(x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : Icons.Material.Filled.CropFree){ MostHavePosition = mhp };
            })];
        }
        else
        {
            InitialTreeItems = [.. (rubrics.Select(x => {
                VerticalDirectionsEnum? mhp = null;

                if(x.SortIndex == min)
                    mhp = VerticalDirectionsEnum.Up;
                else if(x.SortIndex == max)
                    mhp = VerticalDirectionsEnum.Down;

                return new TreeItemDataRubricModel(x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : Icons.Material.Filled.TurnRight){ MostHavePosition = mhp };
            }))];
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
        List<RubricIssueHelpdeskLowModel> res = await RequestRubrics();

        (uint min, uint max) = res.Count(x => x.SortIndex != uint.MaxValue) != 0
            ? (res.Min(x => x.SortIndex), res.Where(x => x.SortIndex != uint.MaxValue).Max(x => x.SortIndex))
            : (0, 0);

        InitialTreeItems = [.. (res.Select(x => {
            VerticalDirectionsEnum? mhp = null;

            if(x.SortIndex == min)
                mhp = VerticalDirectionsEnum.Up;
            else if(x.SortIndex == max)
                mhp = VerticalDirectionsEnum.Down;

            return new TreeItemDataRubricModel(x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : Icons.Material.Filled.TurnRight){ MostHavePosition = mhp };
        }))];
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TreeItemData<RubricIssueHelpdeskLowModel?>>> LoadServerData(RubricIssueHelpdeskLowModel? parentValue)
    {
        ArgumentNullException.ThrowIfNull(parentValue);

        List<RubricIssueHelpdeskLowModel> res = await RequestRubrics(parentValue.Id);
        TreeItemDataRubricModel findNode = FindNode(parentValue.Id, InitialTreeItems) ?? throw new Exception();

        (uint min, uint max) = res.Count(x => x.SortIndex != uint.MaxValue) != 0
            ? (res.Min(x => x.SortIndex), res.Where(x => x.SortIndex != uint.MaxValue).Max(x => x.SortIndex))
            : (0, 0);

        findNode.Children = [.. res.Select(x =>
        {
            VerticalDirectionsEnum? mhp = null;

            if(x.SortIndex == min)
                mhp = VerticalDirectionsEnum.Up;
            else if(x.SortIndex == max)
                mhp = VerticalDirectionsEnum.Down;

            return new TreeItemDataRubricModel(x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : Icons.Material.Filled.TurnRight){ MostHavePosition = mhp };
        })];
        return findNode.Children;
    }

    async Task<List<RubricIssueHelpdeskLowModel>> RequestRubrics(int? parent_id = null)
    {
        IsBusyProgress = true;
        TResponseModel<List<RubricIssueHelpdeskLowModel>?> rest = await HelpdeskRepo.RubricsForIssuesList(new ProjectOwnedRequestModel() { OwnerId = parent_id });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Response is null)
            throw new Exception();

        rest.Response = [.. rest.Response.OrderBy(x => x.SortIndex)];

        rest.Response.Add(new RubricIssueHelpdeskLowModel() { Name = "", SortIndex = uint.MaxValue, ParentRubricId = parent_id });
        return rest.Response;
    }
}