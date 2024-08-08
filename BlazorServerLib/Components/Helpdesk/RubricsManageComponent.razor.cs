////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;
using System.Collections.Generic;

namespace BlazorWebLib.Components.Helpdesk;

/// <inheritdoc/>
public partial class RubricsManageComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    List<TreeItemDataRubricModel> InitialTreeItems { get; set; } = [];

    MudTreeView<RubricIssueHelpdeskBaseModelDB> TreeView_ref = default!;

    /// <inheritdoc/>
    protected override async void OnInitialized()
    {
        IsBusyProgress = true;
        TResponseModel<RubricIssueHelpdeskModelDB[]?> rest = await HelpdeskRepo.GetRubricsIssues(new ProjectOwnedRequestModel());
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Response is null)
            throw new Exception();

        InitialTreeItems.AddRange(rest.Response.Select(x => new TreeItemDataRubricModel(x, Icons.Material.Filled.CarRepair)));
        IsBusyProgress = false;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TreeItemData<RubricIssueHelpdeskBaseModelDB>>?> LoadServerData(RubricIssueHelpdeskBaseModelDB parentValue)
    {
        IsBusyProgress = true;
        TResponseModel<RubricIssueHelpdeskModelDB[]?> rest = await HelpdeskRepo.GetRubricsIssues(new ProjectOwnedRequestModel() { OwnerId = parentValue.Id });
        if (rest.Response is null)
            throw new Exception();

        InitialTreeItems.AddRange(rest.Response.Select(x => new TreeItemDataRubricModel(x, Icons.Material.Filled.CarRepair)));
        IsBusyProgress = false;
        return [..rest.Response.Select(x=>new TreeItemDataRubricModel(x, Icons.Material.Outlined.Lightbulb))];
    }
}