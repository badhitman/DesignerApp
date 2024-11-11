////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

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

    /// <summary>
    /// Id
    /// </summary>
    [Parameter, EditorRequired]
    public required int Id { get; set; }


    WarehouseDocumentModelDB CurrentDocument = new() { DeliveryData = DateTime.Now, Name = "Новый", Rows = [] };
    WarehouseDocumentModelDB editDocument = new() { DeliveryData = DateTime.Now, Name = "Новый", Rows = [] };

    AddRowToOrderDocumentComponent? addingDomRef;
    RowOfWarehouseDocumentModelDB? elementBeforeEdit;

    bool CanSave => Id < 1 || !CurrentDocument.Equals(editDocument);

    async Task SaveDocument()
    {
        await SetBusy();
        TResponseModel<int> res = await commRepo.WarehouseUpdate(editDocument);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (editDocument.Id < 1 && res.Response > 0)
        {
            navRepo.NavigateTo($"/warehouse/editing/{res.Response}");
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (Id < 1)
            return;

        await SetBusy();
        TResponseModel<WarehouseDocumentModelDB[]> res = await commRepo.WarehousesRead([Id]);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success() && res.Response is not null)
            CurrentDocument = res.Response.First();

        editDocument = GlobalTools.CreateDeepCopy(CurrentDocument)!;
        await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override void AddingOfferAction(OfferGoodActionModel off)
    {
        CurrentDocument.Rows ??= [];
        int exist_row = CurrentDocument.Rows.FindIndex(x => x.OfferId == off.Id);
        if (exist_row < 0)
            CurrentDocument.Rows.Add(new()
            {
                Goods = off.Goods,
                GoodsId = off.GoodsId,
                Offer = off,
                OfferId = off.Id,
                WarehouseDocument = CurrentDocument,
                WarehouseDocumentId = CurrentDocument.Id,
                Quantity = off.Quantity,
            });
        else
            CurrentDocument.Rows[exist_row].Quantity = +off.Quantity;

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
    protected override void DeleteRow(int offerId)
    {
        CurrentDocument.Rows ??= [];
        CurrentDocument.Rows.RemoveAll(x => x.OfferId == offerId);
        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();

        addingDomRef?.StateHasChangedCall();
    }
}