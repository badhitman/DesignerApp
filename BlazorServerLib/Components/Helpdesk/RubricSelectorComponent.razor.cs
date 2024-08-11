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
    public required Action<RubricIssueHelpdeskLowModel> SelectedRubricsHandle { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int ParentRubric { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required bool ShowDisabledRubrics { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ModesSelectRubricsEnum ModeSelectingRubrics { get; set; }


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
                InvokeAsync(async () => await childSelector.Reset(_selectedRubricId));

            SelectedRubricsHandle(CurrentRubrics!.First(x => x.Id == _selectedRubricId));
        }
    }

    /// <summary>
    /// Сброс состояния селектора.
    /// </summary>
    public async Task Reset(int parentSet)
    {
        _selectedRubricId = 0;
        IsBusyProgress = true;
        TResponseModel<List<RubricIssueHelpdeskLowModel>?> rest = await HelpdeskRepo.RubricsForIssuesList(new ProjectOwnedRequestModel() { OwnerId = parentSet });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        CurrentRubrics = rest.Response;
    }

#if DEBUG
    static bool IsDebug => true;
#else
    static bool IsDebug => false;
#endif

    RubricSelectorComponent? childSelector;

    List<RubricIssueHelpdeskLowModel>? CurrentRubrics;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await Reset(ParentRubric);
    }
}