////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// Create Issue
/// </summary>
public partial class CreateIssueComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService SerializeStorageRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required Action Update { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<HelpdeskJournalModesEnum> ReloadIssueJournal { get; set; }


    bool CanCreate =>
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(Description) &&
        GlobalTools.DescriptionHtmlToLinesRemark(Description).Any(x => !string.IsNullOrWhiteSpace(x)) &&
        (ModeSelectingRubrics == ModesSelectRubricsEnum.AllowWithoutRubric || (SelectedRubric is not null && SelectedRubric.Id > 0))
        ;

    string? Name;
    string? Description;

    bool IsEditMode { get; set; } = false;

    ModesSelectRubricsEnum ModeSelectingRubrics;
    bool ShowDisabledRubrics;
    RubricIssueHelpdeskLowModel? SelectedRubric;


    async Task CreateIssue()
    {
        IsBusyProgress = true;
        TResponseModel<UserInfoModel?> _current_user = await UsersProfilesRepo.FindByIdAsync();
        if (!_current_user.Success() || _current_user.Response is null)
        {
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(_current_user.Messages);
            return;
        }

        TResponseModel<int> res = await HelpdeskRepo.IssueCreateOrUpdate(new IssueUpdateRequest()
        {
            ActionUserId = _current_user.Response.UserId,
            RubricIssueId = SelectedRubric?.Id,
            Name = Name!,
            Description = Description

        });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success())
            ToggleMode();
        Update();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<bool?> res = await SerializeStorageRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ParameterShowDisabledRubrics);
        TResponseModel<ModesSelectRubricsEnum?> res_ModeSelectingRubrics = await SerializeStorageRepo.ReadParameter<ModesSelectRubricsEnum?>(GlobalStaticConstants.CloudStorageMetadata.ModeSelectingRubrics);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        SnackbarRepo.ShowMessagesResponse(res_ModeSelectingRubrics.Messages);
        if (res_ModeSelectingRubrics.Response is null || (int)res_ModeSelectingRubrics.Response == default)
            res_ModeSelectingRubrics.Response = ModesSelectRubricsEnum.AllowWithoutRubric;

        ShowDisabledRubrics = res.Response == true;
        ModeSelectingRubrics = res_ModeSelectingRubrics.Response.Value;
    }

    void RubricSelectAction(RubricIssueHelpdeskLowModel? selectedRubric)
    {
        SelectedRubric = selectedRubric;
        StateHasChanged();
    }

#if DEBUG
    static bool IsDebug => true;
#else
    static bool IsDebug => false;
#endif

    void ToggleMode()
    {
        IsEditMode = !IsEditMode;
        if (!IsEditMode)
        {
            Description = null;
            Name = null;
            SelectedRubric = null;
            return;
        }
    }
}