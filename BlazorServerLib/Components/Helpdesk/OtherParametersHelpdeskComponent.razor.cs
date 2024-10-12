////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// OtherParametersHelpdeskComponent
/// </summary>
public partial class OtherParametersHelpdeskComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    RubricSelectorComponent? ref_rubric;
    int? _RubricIssueForCreateOrder;
    List<RubricIssueHelpdeskModelDB>? RubricMetadataShadow;
    void RubricSelectAction(RubricBaseModel? selectedRubric)
    {
        _RubricIssueForCreateOrder = selectedRubric?.Id;
        InvokeAsync(SaveRubric);
        StateHasChanged();
    }

    bool _showCreateIssue;
    bool ShowCreateIssue
    {
        get => _showCreateIssue;
        set
        {
            _showCreateIssue = value;
            InvokeAsync(SaveMode);
        }
    }

    async void SaveMode()
    {
        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter<bool?>(ShowCreateIssue, GlobalStaticConstants.CloudStorageMetadata.ShowCreatingIssue, true);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        StateHasChanged();
    }

    async void SaveRubric()
    {
        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter(_RubricIssueForCreateOrder, GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateOrder, true);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<bool?> res_ShowCreatingIssue = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ShowCreatingIssue);
        TResponseModel<int?> res_RubricIssueForCreateOrder = await StorageTransmissionRepo.ReadParameter<int?>(GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateOrder);
        IsBusyProgress = false;
        _RubricIssueForCreateOrder = res_RubricIssueForCreateOrder.Response;
        if (ref_rubric is not null && _RubricIssueForCreateOrder.HasValue)
        {
            IsBusyProgress = true;
            await Task.Delay(1);
            TResponseModel<List<RubricIssueHelpdeskModelDB>?> res = await HelpdeskRepo.RubricRead(_RubricIssueForCreateOrder.Value);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            RubricMetadataShadow = res.Response;
            if (RubricMetadataShadow is not null && RubricMetadataShadow.Count != 0)
            {
                RubricIssueHelpdeskModelDB current_element = RubricMetadataShadow.Last();

                await ref_rubric.OwnerRubricSet(current_element.ParentRubricId ?? 0);
                await ref_rubric.SetRubric(current_element.Id, RubricMetadataShadow);
                ref_rubric.StateHasChangedCall();
            }
        }

        _showCreateIssue = res_ShowCreatingIssue.Success() && res_ShowCreatingIssue.Response == true;
    }
}