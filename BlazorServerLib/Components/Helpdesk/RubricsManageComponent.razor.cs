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
        List<RubricIssueHelpdeskModelDB> rubrics = await RequestRubrics(parent_id);
        IsBusyProgress = false;
        if (parent_id > 0)
        {
            TreeItemDataRubricModel findNode = FindNode(parent_id, InitialTreeItems) ?? throw new Exception();
            findNode.Children = [.. rubrics.Select(x => new TreeItemDataRubricModel(x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : Icons.Material.Filled.CropFree))];
        }
        else
        {
            InitialTreeItems = [.. (rubrics.Select(x => new TreeItemDataRubricModel(x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : Icons.Material.Filled.TurnRight)))];
        }

        StateHasChanged();
    }

    static TreeItemDataRubricModel? FindNode(int parent_id, IEnumerable<TreeItemDataRubricModel> treeItems)
    {
        TreeItemDataRubricModel? res = treeItems.FirstOrDefault(x => x.Value?.Id == parent_id);
        if (res is not null)
            return res;

        TreeItemDataRubricModel? FindChildNode(List<TreeItemData<RubricIssueHelpdeskBaseModelDB?>> children)
        {
            TreeItemData<RubricIssueHelpdeskBaseModelDB?>? res_child = children.FirstOrDefault(x => x.Value?.Id == parent_id);
            if (res_child is not null)
                return (TreeItemDataRubricModel?)res_child;

            foreach (TreeItemData<RubricIssueHelpdeskBaseModelDB?> c in children)
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
        List<RubricIssueHelpdeskModelDB> res = await RequestRubrics();
        InitialTreeItems = [.. (res.Select(x => new TreeItemDataRubricModel(x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : Icons.Material.Filled.TurnRight)))];
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TreeItemData<RubricIssueHelpdeskBaseModelDB?>>> LoadServerData(RubricIssueHelpdeskBaseModelDB? parentValue)
    {
        ArgumentNullException.ThrowIfNull(parentValue);

        List<RubricIssueHelpdeskModelDB> res = await RequestRubrics(parentValue.Id);
        TreeItemDataRubricModel findNode = FindNode(parentValue.Id, InitialTreeItems) ?? throw new Exception();

        findNode.Children = [.. res.Select(x => new TreeItemDataRubricModel(x, x.Id == 0 ? Icons.Material.Filled.PlaylistAdd : Icons.Material.Filled.TurnRight))];
        return findNode.Children;
    }

    async Task<List<RubricIssueHelpdeskModelDB>> RequestRubrics(int? parent_id = null)
    {
        IsBusyProgress = true;
        TResponseModel<List<RubricIssueHelpdeskModelDB>?> rest = await HelpdeskRepo.RubricsForIssuesList(new ProjectOwnedRequestModel() { OwnerId = parent_id });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Response is null)
            throw new Exception();

        rest.Response.Add(new RubricIssueHelpdeskModelDB() { Name = "", SortIndex = uint.MaxValue, ParentRubricId = parent_id });
        return rest.Response;
    }
}