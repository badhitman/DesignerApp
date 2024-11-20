////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Helpdesk;
using BlazorWebLib.Components.Commerce;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Warehouse;

/// <summary>
/// WarehouseEditingComponent
/// </summary>
public partial class WarehouseEditingComponent : OffersTableBaseComponent
{
    [Inject]
    ICommerceRemoteTransmissionService commRepo { get; set; } = default!;

    [Inject]
    NavigationManager navRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <summary>
    /// Id
    /// </summary>
    [Parameter, EditorRequired]
    public required int Id { get; set; }


    WarehouseDocumentModelDB CurrentDocument = new() { DeliveryDate = DateTime.Now, Name = "Новый", NormalizedUpperName = "НОВЫЙ", Rows = [] };
    WarehouseDocumentModelDB editDocument = new() { DeliveryDate = DateTime.Now, Name = "Новый", NormalizedUpperName = "НОВЫЙ", Rows = [] };
    RubricSelectorComponent? ref_rubric;
    AddRowToOrderDocumentComponent? addingDomRef;
    RowOfWarehouseDocumentModelDB? elementBeforeEdit;
    List<RubricIssueHelpdeskModelDB>? RubricMetadataShadow;

    bool CanSave => Id < 1 || !CurrentDocument.Equals(editDocument);

    void RubricSelectAction(RubricBaseModel? selectedRubric)
    {
        editDocument.WarehouseId = selectedRubric?.Id ?? 0;
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async void RowEditCommitHandler(object element)
    {
        if (element is RowOfWarehouseDocumentModelDB _el)
        {
            TResponseModel<int> res = await commRepo.RowForWarehouseUpdate(_el);
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        }
        await ReadDocument();
        base.RowEditCommitHandler(element);
    }

    async Task SaveDocument()
    {
        await SetBusy();
        TResponseModel<int> res = await commRepo.WarehouseUpdate(editDocument);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (editDocument.Id < 1 && res.Response > 0)
            navRepo.NavigateTo($"/goods/warehouse/editing/{res.Response}");
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetBusy();
        if (Id < 1)
        {
            TResponseModel<List<RubricIssueHelpdeskModelDB>?> res = await HelpdeskRepo.RubricRead(0);
            await SetBusy(false);
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            RubricMetadataShadow = res.Response;
            if (RubricMetadataShadow is not null && RubricMetadataShadow.Count != 0)
            {
                RubricIssueHelpdeskModelDB current_element = RubricMetadataShadow.Last();
                if (ref_rubric is not null)
                {
                    await ref_rubric.OwnerRubricSet(current_element.ParentRubricId ?? 0);
                    await ref_rubric.SetRubric(current_element.Id, RubricMetadataShadow);
                    ref_rubric.StateHasChangedCall();
                }
            }
            return;
        }

        await ReadDocument();
    }

    async Task ReadDocument()
    {
        await SetBusy();
        TResponseModel<WarehouseDocumentModelDB[]> res = await commRepo.WarehousesRead([Id]);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success() && res.Response is not null)
            CurrentDocument = res.Response.First();

        editDocument = GlobalTools.CreateDeepCopy(CurrentDocument)!;

        TResponseModel<List<RubricIssueHelpdeskModelDB>?> resShadow = await HelpdeskRepo.RubricRead(editDocument.WarehouseId);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(resShadow.Messages);
        RubricMetadataShadow = resShadow.Response;
        if (RubricMetadataShadow is not null && RubricMetadataShadow.Count != 0)
        {
            RubricIssueHelpdeskModelDB current_element = RubricMetadataShadow.Last();
            if (ref_rubric is not null)
            {
                await ref_rubric.OwnerRubricSet(current_element.ParentRubricId ?? 0);
                await ref_rubric.SetRubric(current_element.Id, RubricMetadataShadow);
                ref_rubric.StateHasChangedCall();
            }
        }
    }

    /// <inheritdoc/>
    protected override async void AddingOfferAction(OfferGoodActionModel off)
    {
        CurrentDocument.Rows ??= [];
        int exist_row = CurrentDocument.Rows.FindIndex(x => x.OfferId == off.Id);
        TResponseModel<int> res;
        if (exist_row < 0)
        {
            RowOfWarehouseDocumentModelDB _newRow = new()
            {
                GoodsId = off.GoodsId,
                OfferId = off.Id,
                WarehouseDocumentId = CurrentDocument.Id,
                Quantity = off.Quantity,
            };

            await SetBusy();
            res = await commRepo.RowForWarehouseUpdate(_newRow);
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            await SetBusy(false);
            if (!res.Success())
                return;

            addingDomRef?.StateHasChangedCall();
            if (DocumentUpdateHandler is not null)
                DocumentUpdateHandler();
        }
        else
        {
            CurrentDocument.Rows[exist_row].Quantity = +off.Quantity;
            await SetBusy();
            res = await commRepo.RowForWarehouseUpdate(CurrentDocument.Rows[exist_row]);
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            await SetBusy(false);
        }

        await ReadDocument();
        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();

        StateHasChanged();
        addingDomRef!.StateHasChangedCall();
    }

    /// <inheritdoc/>
    protected override void RowEditPreviewHandler(object element)
        => elementBeforeEdit = GlobalTools.CreateDeepCopy((RowOfWarehouseDocumentModelDB)element);

    /// <inheritdoc/>
    protected override void RowEditCancelHandler(object element)
    {
        ((RowOfWarehouseDocumentModelDB)element).Quantity = elementBeforeEdit!.Quantity;
        elementBeforeEdit = null;
    }

    /// <inheritdoc/>
    protected override async void DeleteRow(int offerId)
    {
        CurrentDocument.Rows ??= [];
        RowOfWarehouseDocumentModelDB? currentRow = CurrentDocument.Rows.FirstOrDefault(x => x.OfferId == offerId);
        if (currentRow is null)
        {
            await ReadDocument();
            return;
        }

        if (currentRow.Id < 1)
        {
            CurrentDocument.Rows.RemoveAll(x => x.OfferId == offerId);
            return;
        }

        await SetBusy();
        TResponseModel<bool> res = await commRepo.RowsForWarehouseDelete([currentRow.Id]);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await SetBusy(false);
        if (!res.Success())
            return;

        await ReadDocument();
        addingDomRef?.StateHasChangedCall();
        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();
    }
}