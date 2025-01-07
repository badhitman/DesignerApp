////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Helpdesk;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.ParametersShared;

/// <summary>
/// RubricParameterStorageComponent
/// </summary>
public partial class RubricParameterStorageComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StoreRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <summary>
    /// Label
    /// </summary>
    [Parameter, EditorRequired]
    public required string Label { get; set; }

    /// <summary>
    /// KeyStorage
    /// </summary>
    [Parameter, EditorRequired]
    public required StorageMetadataModel KeyStorage { get; set; }

    /// <summary>
    /// HelperText
    /// </summary>
    [Parameter]
    public string? HelperText { get; set; }

    /// <summary>
    /// ModeSelectingRubrics
    /// </summary>
    [Parameter]
    public ModesSelectRubricsEnum ModeSelectingRubrics { get; set; } = ModesSelectRubricsEnum.AllowWithoutRubric;

    /// <summary>
    /// ShowDisabledRubrics
    /// </summary>
    [Parameter]
    public bool ShowDisabledRubrics { get; set; } = true;


    RubricSelectorComponent? ref_rubric;
    int? _rubricSelected;
    List<RubricIssueHelpdeskModelDB>? RubricMetadataShadow;
    void RubricSelectAction(UniversalBaseModel? selectedRubric)
    {
        _rubricSelected = selectedRubric?.Id;
        InvokeAsync(SaveRubric);
        StateHasChanged();
    }

    async void SaveRubric()
    {
        await SetBusy();
        TResponseModel<int> res = await StoreRepo.SaveParameter(_rubricSelected, KeyStorage, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        TResponseModel<int?> res_RubricIssueForCreateOrder = await StoreRepo.ReadParameter<int?>(KeyStorage);
        _rubricSelected = res_RubricIssueForCreateOrder.Response;
        if (ref_rubric is not null && _rubricSelected.HasValue)
        {
            TResponseModel<List<RubricIssueHelpdeskModelDB>> res = await HelpdeskRepo.RubricRead(_rubricSelected.Value);
            await SetBusy(false);
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            RubricMetadataShadow = res.Response;
            if (RubricMetadataShadow is not null && RubricMetadataShadow.Count != 0)
            {
                RubricIssueHelpdeskModelDB current_element = RubricMetadataShadow.Last();

                await ref_rubric.OwnerRubricSet(current_element.ParentId ?? 0);
                await ref_rubric.SetRubric(current_element.Id, RubricMetadataShadow);
                ref_rubric.StateHasChangedCall();
            }
        }
        else
            await SetBusy(false);
    }
}