﻿////////////////////////////////////////////////
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
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService SerializeStorageRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<HelpdeskJournalModesEnum> ReloadIssueJournal { get; set; }


    bool CanCreate =>
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
        TResponseModel<int> res = await HelpdeskRepo.IssueCreateOrUpdate(new IssueHelpdeskModelDB()
        {
            AuthorIdentityUserId = "",
            RubricIssueId = SelectedRubric?.Id,
            LastUpdateAt = DateTime.Now,
            Name = "",
            Description = Description,
            ExecutorIdentityUserId = "",
            StepIssue = HelpdeskIssueStepsEnum.Created,

        });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
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

    void RubricSelectAction(RubricIssueHelpdeskLowModel selectedRubric)
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
            return;
        }
    }
}