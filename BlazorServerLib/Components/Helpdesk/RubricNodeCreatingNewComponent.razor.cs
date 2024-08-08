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
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int ParentRubricId { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required RubricsManageComponent HelpdeskParentView { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<int> ReloadNodeHandle { get; set; }


    RubricIssueHelpdeskBaseModelDB ItemModel = default!;

    string? rubricName;

    /// <inheritdoc/>
    protected string DomID => $"rubric-create-for-{ParentRubricId}";

    bool IsEdit => !string.IsNullOrWhiteSpace(rubricName);

    async Task RubricCreateNew()
    {
        if (string.IsNullOrWhiteSpace(rubricName))
            throw new Exception();

        IsBusyProgress = true;
        TResponseModel<int?> rest = await HelpdeskRepo.RubricForIssuesCreateOrUpdate(new() { Name = rubricName, ParentRubricId = ItemModel.Id > 0 ? ItemModel.Id : null });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        
        ReloadNodeHandle(0);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ItemModel = new RubricIssueHelpdeskBaseModelDB() { Name = "", ParentRubricId = ParentRubricId };
    }
}