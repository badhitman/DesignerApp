////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// RubricNode: Edit
/// </summary>
public partial class RubricNodeEditComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <summary>
    /// ReadOnly
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<int> ReloadNodeHandle { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<RubricBaseModel> ItemUpdateHandle { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required TreeItemDataRubricModel Item { get; set; }


    bool IsRenameMode;

    RubricBaseModel? ItemModel;

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

        await SetBusy();
        TResponseModel<bool?> res = await HelpdeskRepo.RubricMove(new RowMoveModel() { Direction = dir, ObjectId = rubric.Value!.Id });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        ReloadNodeHandle(ItemModel.ParentId ?? 0);
    }

    async Task SaveRubric()
    {
        if (ItemModel is null)
            throw new ArgumentNullException(nameof(ItemModel));

        if (string.IsNullOrWhiteSpace(itemSystemName))
            throw new ArgumentNullException(nameof(itemSystemName));
        
        IsRenameMode = false;
        ItemModel.Name = itemSystemName;

        await SetBusy();
        TResponseModel<int?> res = await HelpdeskRepo.RubricCreateOrUpdate(new RubricIssueHelpdeskModelDB()
        {
            Name = ItemModel.Name,
            Description = ItemModel.Description,
            Id = ItemModel.Id,
            ParentId = ItemModel.ParentId,
            ProjectId = ItemModel.ProjectId,
            SortIndex = ItemModel.SortIndex,
            IsDisabled = ItemModel.IsDisabled,
        });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        ItemUpdateHandle(ItemModel);
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