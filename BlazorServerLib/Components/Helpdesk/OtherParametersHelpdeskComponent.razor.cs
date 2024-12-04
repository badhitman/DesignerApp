////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// OtherParametersHelpdeskComponent
/// </summary>
public partial class OtherParametersHelpdeskComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    RubricSelectorComponent? ref_rubric;
    int? _rubricIssueForCreateOrder;
    List<RubricIssueHelpdeskModelDB>? RubricMetadataShadow;
    void RubricSelectAction(UniversalBaseModel? selectedRubric)
    {
        _rubricIssueForCreateOrder = selectedRubric?.Id;
        InvokeAsync(SaveRubric);
        StateHasChanged();
    }

    async void SaveRubric()
    {
        await SetBusy();
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter(_rubricIssueForCreateOrder, GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateOrder, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        TResponseModel<int?> res_RubricIssueForCreateOrder = await StorageTransmissionRepo.ReadParameter<int?>(GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateOrder);
        _rubricIssueForCreateOrder = res_RubricIssueForCreateOrder.Response;
        if (ref_rubric is not null && _rubricIssueForCreateOrder.HasValue)
        {
            TResponseModel<List<RubricIssueHelpdeskModelDB>?> res = await HelpdeskRepo.RubricRead(_rubricIssueForCreateOrder.Value);
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