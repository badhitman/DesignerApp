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
    void RubricSelectAction(RubricBaseModel? selectedRubric)
    {
        _rubricIssueForCreateOrder = selectedRubric?.Id;
        InvokeAsync(SaveRubric);
        StateHasChanged();
    }

    bool _showCreatingIssue;
    bool ShowCreatingIssue
    {
        get => _showCreatingIssue;
        set
        {
            _showCreatingIssue = value;
            InvokeAsync(SaveModeShowCreatingIssue);
        }
    }

    bool _showingTelegramArea;
    bool ShowingTelegramArea
    {
        get => _showingTelegramArea;
        set
        {
            _showingTelegramArea = value;
            InvokeAsync(SaveModeShowingTelegramAreaIssue);
        }
    }

    bool _showingPriceSelectorOrder;
    bool ShowingPriceSelectorOrder
    {
        get => _showingPriceSelectorOrder;
        set
        {
            _showingPriceSelectorOrder = value;
            InvokeAsync(SaveModeShowingPriceSelector);
        }
    }

    bool _showingAttachmentsOrderArea;
    bool ShowingAttachmentsOrderArea
    {
        get => _showingAttachmentsOrderArea;
        set
        {
            _showingAttachmentsOrderArea = value;
            InvokeAsync(SaveModeShowingAttachmentsOrderArea);
        }
    }

    bool _showingAttachmentsIssueArea;
    bool ShowingAttachmentsIssueArea
    {
        get => _showingAttachmentsIssueArea;
        set
        {
            _showingAttachmentsIssueArea = value;
            InvokeAsync(SaveModeShowingAttachmentsIssueArea);
        }
    }


    async void SaveModeShowCreatingIssue()
    {
        await SetBusy();
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter<bool?>(ShowCreatingIssue, GlobalStaticConstants.CloudStorageMetadata.ShowCreatingIssue, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    async void SaveModeShowingTelegramAreaIssue()
    {
        await SetBusy();
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter<bool?>(ShowingTelegramArea, GlobalStaticConstants.CloudStorageMetadata.ShowingTelegramArea, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    async void SaveModeShowingAttachmentsOrderArea()
    {
        await SetBusy();
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter<bool?>(ShowCreatingIssue, GlobalStaticConstants.CloudStorageMetadata.ShowingAttachmentsOrderArea, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    async void SaveModeShowingPriceSelector()
    {
        await SetBusy();
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter<bool?>(ShowingAttachmentsOrderArea, GlobalStaticConstants.CloudStorageMetadata.ShowingPriceSelectorOrder, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }
    // 
    async void SaveModeShowingAttachmentsIssueArea()
    {
        await SetBusy();
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter<bool?>(ShowCreatingIssue, GlobalStaticConstants.CloudStorageMetadata.ShowingAttachmentsIssuesArea, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
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

        TResponseModel<bool?> showCreatingIssue = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ShowCreatingIssue);
        _showCreatingIssue = showCreatingIssue.Success() && showCreatingIssue.Response == true;




        TResponseModel<int?> res_RubricIssueForCreateOrder = await StorageTransmissionRepo.ReadParameter<int?>(GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateOrder);
        _rubricIssueForCreateOrder = res_RubricIssueForCreateOrder.Response;
        IsBusyProgress = false;
        if (ref_rubric is not null && _rubricIssueForCreateOrder.HasValue)
        {
            await SetBusy();
            TResponseModel<List<RubricIssueHelpdeskModelDB>?> res = await HelpdeskRepo.RubricRead(_rubricIssueForCreateOrder.Value);
            await SetBusy(false);
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
    }
}