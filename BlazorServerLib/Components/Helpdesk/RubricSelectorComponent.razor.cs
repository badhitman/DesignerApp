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
        IsBusyProgress = true;
        TResponseModel<List<RubricIssueHelpdeskLowModel>?> rest = await HelpdeskRepo.RubricsList(new ProjectOwnedRequestModel() { OwnerId = ownerRubricId });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        CurrentRubrics = rest.Response;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await OwnerRubricSet(ParentRubric);
    }
}