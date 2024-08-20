﻿////////////////////////////////////////////////
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
    ISerializeStorageRemoteTransmissionService SerializeStorageRepo { get; set; } = default!;


    bool CanSave =>
        !string.IsNullOrWhiteSpace(NameIssueEdit) &&
        !string.IsNullOrWhiteSpace(DescriptionIssueEdit) &&
        GlobalTools.DescriptionHtmlToLinesRemark(DescriptionIssueEdit).Any(x => !string.IsNullOrWhiteSpace(x)) &&
        (ModeSelectingRubrics == ModesSelectRubricsEnum.AllowWithoutRubric || (SelectedRubric is not null && SelectedRubric.Id > 0))
        ;

    string? NameIssueEdit { get; set; }
    string? DescriptionIssueEdit { get; set; }

    int? RubricIssueEdit { get; set; }

    bool IsEdited =>
        NameIssueEdit != Issue.Name ||
        DescriptionIssueEdit != Issue.Description ||
        RubricIssueEdit != Issue.RubricIssueId;

    MarkupString DescriptionHtml => (MarkupString)(Issue.Description ?? "");

    ModesSelectRubricsEnum ModeSelectingRubrics;
    bool ShowDisabledRubrics;
    bool IsEditMode { get; set; }
    RubricIssueHelpdeskLowModel? SelectedRubric;

    void RubricSelectAction(RubricIssueHelpdeskLowModel? selectedRubric)
    {
        SelectedRubric = selectedRubric;
        RubricIssueEdit = SelectedRubric?.Id;
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
        if (string.IsNullOrWhiteSpace(NameIssueEdit))
            throw new ArgumentNullException(nameof(NameIssueEdit));

        IsBusyProgress = true;
        
        TResponseModel<int> res = await HelpdeskRepo.IssueCreateOrUpdate(new()
        {
            SenderActionUserId = CurrentUser.UserId,
            Payload = new()
            {
                Name = NameIssueEdit,
                Description = DescriptionIssueEdit,
                RubricId = RubricIssueEdit,
                Id = Issue.Id,
            }
        });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        IsBusyProgress = false;
        if (!res.Success())
            return;

        Issue.Name = NameIssueEdit;
        Issue.Description = DescriptionIssueEdit;
        Issue.RubricIssueId = RubricIssueEdit;

        IsEditMode = false;
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
        SelectedRubric = Issue.RubricIssue;
    }
}