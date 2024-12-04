////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// RubricNode: Creating New
/// </summary>
public partial class RubricNodeCreatingNewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required RubricsManageComponent HelpdeskParentView { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<int> ReloadNodeHandle { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required TreeItemData<UniversalBaseModel> Item { get; set; }

    /// <summary>
    /// Имя контекста
    /// </summary>
    [Parameter]
    public string? ContextName { get; set; }


    UniversalBaseModel ItemModel = default!;

    string? rubricName;

    /// <inheritdoc/>
    protected string DomID => $"rubric-create-for-{Item.Value?.ParentId}";

    bool IsEdit => !string.IsNullOrWhiteSpace(rubricName);

    async Task RubricCreateNew()
    {
        if (string.IsNullOrWhiteSpace(rubricName))
            throw new Exception();

        await SetBusy();
        TResponseModel<int?> rest = await HelpdeskRepo.RubricCreateOrUpdate(new() { Name = rubricName, ParentId = ItemModel.ParentId > 0 ? ItemModel.ParentId : null, ContextName = ContextName });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        ReloadNodeHandle(Item.Value?.ParentId ?? 0);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ItemModel = new RubricIssueHelpdeskMiddleModel()
        {
            Name = "",
            ParentId = Item.Value?.ParentId,
            ContextName = ContextName,
        };
    }
}