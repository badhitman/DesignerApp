////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
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


    /// <summary>
    /// IssueSource
    /// </summary>
    [Parameter, CascadingParameter]
    public IssueHelpdeskModelDB? IssueSource { get; set; }

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
    RubricBaseModel? SelectedRubric;
    RubricSelectorComponent? rubricSelector_ref;
    List<RubricIssueHelpdeskModelDB>? RubricMetadataShadow;

    void RubricSelectAction(RubricBaseModel? selectedRubric)
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

        await SetBusy();

        TResponseModel<int> res = await HelpdeskRepo.IssueCreateOrUpdate(new()
        {
            SenderActionUserId = CurrentUserSession!.UserId,
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

    async Task EditToggle()
    {
        IsEditMode = !IsEditMode;

        if (rubricSelector_ref is not null && Issue.RubricIssueId is not null)
        {
            await SetBusy();

            TResponseModel<List<RubricIssueHelpdeskModelDB>?> res = await HelpdeskRepo.RubricRead(Issue.RubricIssueId.Value);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            RubricMetadataShadow = res.Response;
            if (RubricMetadataShadow is not null && RubricMetadataShadow.Count != 0)
            {
                RubricIssueHelpdeskModelDB current_element = RubricMetadataShadow.Last();

                await rubricSelector_ref.OwnerRubricSet(current_element.ParentRubricId ?? 0);
                await rubricSelector_ref.SetRubric(current_element.Id, RubricMetadataShadow);
                rubricSelector_ref.StateHasChangedCall();
            }
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        CancelEdit();

        await SetBusy();
        TResponseModel<bool?> res = await SerializeStorageRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ParameterShowDisabledRubrics);
        TResponseModel<ModesSelectRubricsEnum?> res_ModeSelectingRubrics = await SerializeStorageRepo.ReadParameter<ModesSelectRubricsEnum?>(GlobalStaticConstants.CloudStorageMetadata.ModeSelectingRubrics);
        IsBusyProgress = false;

        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (!res_ModeSelectingRubrics.Success())
            SnackbarRepo.ShowMessagesResponse(res_ModeSelectingRubrics.Messages);

        if (res_ModeSelectingRubrics.Response is null || (int)res_ModeSelectingRubrics.Response == 0)
            res_ModeSelectingRubrics.Response = ModesSelectRubricsEnum.AllowWithoutRubric;

        ShowDisabledRubrics = res.Response == true;
        ModeSelectingRubrics = res_ModeSelectingRubrics.Response.Value;
        SelectedRubric = Issue.RubricIssue;
    }
}