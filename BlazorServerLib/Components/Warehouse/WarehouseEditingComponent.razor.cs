////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Commerce;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Warehouse;

/// <summary>
/// WarehouseEditingComponent
/// </summary>
public partial class WarehouseEditingComponent : OffersTableBaseComponent
{
    /// <summary>
    /// Id
    /// </summary>
    [Parameter, EditorRequired]
    public required int Id { get; set; }


    WarehouseDocumentModelDB CurrentDocument = new() { DeliveryData = DateTime.Now, Name = "Новый", Rows = [] };
    AddRowToOrderDocumentComponent? addingDomRef;
    RowOfWarehouseDocumentModelDB? elementBeforeEdit;

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