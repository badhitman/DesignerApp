////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// RubricNode: Edit
/// </summary>
public partial class RubricNodeEditComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<int> ReloadNodeHandle { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required TreeItemData<RubricIssueHelpdeskBaseModelDB> Item { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required RubricsManageComponent HelpdeskParentView { get; set; }


    bool IsRenameMode;

    RubricIssueHelpdeskBaseModelDB? ItemModel;

    string? itemSystemName;

    /// <inheritdoc/>
    protected string DomID => $"{Item.Value?.Id}";

    bool IsEditedName => itemSystemName != ItemModel?.Name;

    async Task SaveRubric()
    {
        if (ItemModel is null)
            throw new ArgumentNullException(nameof(ItemModel));

        if(string.IsNullOrWhiteSpace(itemSystemName))
            throw new ArgumentNullException(nameof(itemSystemName));

        ItemModel.Name = itemSystemName;
        
        IsBusyProgress = true;
        TResponseModel<int?> res = await HelpdeskRepo.RubricForIssuesCreateOrUpdate((RubricIssueHelpdeskModelDB)ItemModel);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        ReloadNodeHandle(ItemModel.ParentRubricId ?? 0);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ItemModel = Item.Value;
        itemSystemName = ItemModel?.Name;
    }

    /// <inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        bool need_refresh = ItemModel != Item.Value;
        ItemModel = Item.Value;
        if (need_refresh)
            StateHasChanged();
    }
}