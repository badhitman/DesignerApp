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
    [Parameter, EditorRequired]
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

    async Task ToggleDisabled()
    {
        if (ItemModel is null)
            throw new ArgumentNullException(nameof(ItemModel));

        RubricIssueHelpdeskModelDB _upd = new()
        {
            Name = ItemModel.Name,
            Id = ItemModel.Id,
            Description = ItemModel.Description,
            IsDisabled = !ItemModel.IsDisabled,
            ParentRubricId = ItemModel.ParentRubricId,
            SortIndex = ItemModel.SortIndex,
            ProjectId = ItemModel.ProjectId
        };



        IsBusyProgress = true;
        var res = await HelpdeskRepo.RubricForIssuesCreateOrUpdate(_upd);
        IsBusyProgress = false;
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ItemModel = Item.Value;
        itemSystemName = ItemModel?.Name;
    }
}