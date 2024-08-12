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
    public required TreeItemDataRubricModel Item { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required RubricsManageComponent HelpdeskParentView { get; set; }


    bool IsRenameMode;

    RubricIssueHelpdeskLowModel? ItemModel;

    string? itemSystemName;

    /// <inheritdoc/>
    protected string DomID => $"{Item.Value?.Id}";

    bool IsEditedName => itemSystemName != ItemModel?.Name;

    //Item.MoveRowState
    static MoveRowStatesEnum[] CantUpMove => [MoveRowStatesEnum.Start, MoveRowStatesEnum.Singleton];

    static MoveRowStatesEnum[] CantDownMove => [MoveRowStatesEnum.End, MoveRowStatesEnum.Singleton];

    async Task MoveRow(VerticalDirectionsEnum dir, TreeItemDataRubricModel rubric)
    {
        if (ItemModel is null)
            throw new ArgumentNullException(nameof(ItemModel));

        IsBusyProgress = true;
        TResponseModel<bool?> res = await HelpdeskRepo.RubricForIssuesMove(new RowMoveModel() { Direction = dir, ObjectId = rubric.Value!.Id });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        ReloadNodeHandle(ItemModel.ParentRubricId ?? 0);
    }

    async Task SaveRubric()
    {
        if (ItemModel is null)
            throw new ArgumentNullException(nameof(ItemModel));

        if (string.IsNullOrWhiteSpace(itemSystemName))
            throw new ArgumentNullException(nameof(itemSystemName));

        ItemModel.Name = itemSystemName;

        IsBusyProgress = true;
        TResponseModel<int?> res = await HelpdeskRepo.RubricForIssuesCreateOrUpdate(new RubricIssueHelpdeskModelDB()
        {
            Name = ItemModel.Name,
            Description = ItemModel.Description,
            Id = ItemModel.Id,
            ParentRubricId = ItemModel.ParentRubricId,
            ProjectId = ItemModel.ProjectId,
            SortIndex = ItemModel.SortIndex,
            IsDisabled = ItemModel.IsDisabled,
        });
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