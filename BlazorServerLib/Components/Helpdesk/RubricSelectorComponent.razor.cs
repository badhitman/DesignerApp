////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// Rubric selector
/// </summary>
public partial class RubricSelectorComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<RubricIssueHelpdeskLowModel?> SelectedRubricsHandle { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int ParentRubric { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required bool ShowDisabledRubrics { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ModesSelectRubricsEnum ModeSelectingRubrics { get; set; }

    /// <summary>
    /// Owner issue
    /// </summary>
    [CascadingParameter]
    public IssueHelpdeskModelDB? IssueSource { get; set; }

    [CascadingParameter]
    List<RubricIssueHelpdeskModelDB>? RubricMetadataShadow { get; set; }


    RubricSelectorComponent? childSelector;

    List<RubricIssueHelpdeskLowModel>? CurrentRubrics;

    int _selectedRubricId;
    /// <summary>
    /// Selected Rubric Id
    /// </summary>
    int SelectedRubricId
    {
        get => _selectedRubricId;
        set
        {
            _selectedRubricId = value;
            if (childSelector is not null)
                InvokeAsync(async () => await childSelector.OwnerRubricSet(_selectedRubricId));
            SelectedRubricsHandle(_selectedRubricId > 0 ? CurrentRubrics!.First(x => x.Id == _selectedRubricId) : null);
        }
    }

    /// <summary>
    /// Сброс состояния селектора.
    /// </summary>
    public async Task OwnerRubricSet(int ownerRubricId)
    {
        if(ParentRubric != ownerRubricId)
        {
            ParentRubric = ownerRubricId;
            _selectedRubricId = 0;
        }

        IsBusyProgress = true;
        TResponseModel<List<RubricIssueHelpdeskLowModel>?> rest = await HelpdeskRepo.RubricsList(new TProjectedRequestModel<int>() { Request = ownerRubricId });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        CurrentRubrics = rest.Response;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await OwnerRubricSet(ParentRubric);
        if (ParentRubric == 0 && IssueSource is not null && IssueSource.RubricIssueId.HasValue)
        {
            IsBusyProgress = true;
            TResponseModel<List<RubricIssueHelpdeskModelDB>?> dump_rubric = await HelpdeskRepo.RubricRead(IssueSource.RubricIssueId.Value);
            RubricMetadataShadow = dump_rubric.Response;
            SnackbarRepo.ShowMessagesResponse(dump_rubric.Messages);
            IsBusyProgress = false;
            _selectedRubricId = RubricMetadataShadow?.LastOrDefault()?.Id ?? 0;
        }
        else if (RubricMetadataShadow is not null && RubricMetadataShadow.Count != 0)
        {
            _selectedRubricId = RubricMetadataShadow?.LastOrDefault()?.Id ?? 0;
        }
    }
}