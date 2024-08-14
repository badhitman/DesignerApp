////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;
using BlazorLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// IssueBodyComponent
/// </summary>
public partial class IssueBodyComponent : IssueWrapBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService SerializeStorageRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;


    string? NameIssueEdit { get; set; }
    string? DescriptionIssueEdit { get; set; }
    
    int? RubricIssueEdit { get; set; }

    bool IsEdited => NameIssueEdit != Issue.Name || DescriptionIssueEdit != Issue.Description || RubricIssueEdit != Issue.RubricIssueId;

    MarkupString DescriptionHtml => (MarkupString)(Issue.Description ?? "");

    ModesSelectRubricsEnum ModeSelectingRubrics;
    bool ShowDisabledRubrics;
    bool IsEditMode { get; set; }
    RubricIssueHelpdeskLowModel? SelectedRubric;

    void RubricSelectAction(RubricIssueHelpdeskLowModel? selectedRubric)
    {
        SelectedRubric = selectedRubric;
        StateHasChanged();
    }

    void CancelEdit()
    {
        NameIssueEdit = Issue.Name;
        DescriptionIssueEdit = Issue.Description;
        RubricIssueEdit = Issue.RubricIssueId;

        IsEditMode = false;
    }

    async Task SaveIssue()
    {
        IsBusyProgress = true;
        TResponseModel<UserInfoModel?> _current_user = await UsersProfilesRepo.FindByIdAsync();
        if (!_current_user.Success() || _current_user.Response is null)
        {
            SnackbarRepo.ShowMessagesResponse(_current_user.Messages);
            return;
        }

        //var res = await HelpdeskRepo.IssueCreateOrUpdate(new () { AuthorIdentityUserId });
        IsBusyProgress = false;
    }

    void EditToggle()
    {
        IsEditMode = !IsEditMode;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        CancelEdit();

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
}