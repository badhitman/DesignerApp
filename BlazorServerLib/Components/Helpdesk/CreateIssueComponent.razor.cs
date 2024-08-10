////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// Create Issue
/// </summary>
public partial class CreateIssueComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService SerializeStorageRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<HelpdeskJournalModesEnum> ReloadIssueJournal { get; set; }

    string? Description;

    bool IsEditMode { get; set; } = false;

    bool RequiredRubric;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        StorageCloudParameterReadModel par = new()
        {
            ApplicationName = GlobalStaticConstants.Routes.HELPDESK_CONTROLLER_NAME,
            Name = Path.Combine(GlobalStaticConstants.Routes.RUBRIC_CONTROLLER_NAME, GlobalStaticConstants.Routes.CONFIGURATION_CONTROLLER_NAME),
            TypeName = Path.Combine(GlobalStaticConstants.Routes.RUBRIC_CONTROLLER_NAME, GlobalStaticConstants.Routes.FORM_CONTROLLER_NAME),
        };
        IsBusyProgress = true;
        TResponseModel<bool?> res = await SerializeStorageRepo.ReadParameter<bool?>(par);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        RequiredRubric = res.Response == true;
    }

    void ToggleMode()
    {
        IsEditMode = !IsEditMode;
        if (!IsEditMode)
            return;


    }
}