////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// TabAddressOfOrderDocumentComponent
/// </summary>
public partial class TabAddressOfOrderDocumentComponent : OffersTableBaseComponent
{
    /// <summary>
    /// CurrentTab
    /// </summary>
    [Parameter, EditorRequired]
    public required TabAddressForOrderModelDb CurrentTab { get; set; }


    AddRowToOrderDocumentComponent? addingDomRef;
    RowOfOrderDocumentModelDB? elementBeforeEdit;


    /// <inheritdoc/>
    protected override void DeleteRow(int offerId)
    {
        CurrentTab.Rows ??= [];
        CurrentTab.Rows.RemoveAll(x => x.OfferId == offerId);
        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();

        addingDomRef?.StateHasChangedCall();
    }

    /// <inheritdoc/>
    protected override void AddingOfferAction(OfferGoodActionModel off)
    {
        CurrentTab.Rows ??= [];
        int exist_row = CurrentTab.Rows.FindIndex(x => x.OfferId == off.Id);
        if (exist_row < 0)
            CurrentTab.Rows.Add(new RowOfOrderDocumentModelDB()
            {
                AddressForOrderTab = CurrentTab,
                AddressForOrderTabId = CurrentTab.Id,
                Goods = off.Goods,
                GoodsId = off.GoodsId,
                Offer = off,
                OfferId = off.Id,
                OrderDocument = CurrentTab.OrderDocument,
                OrderDocumentId = CurrentTab.OrderDocumentId,
                Quantity = off.Quantity,
            });
        else
            CurrentTab.Rows[exist_row].Quantity = +off.Quantity;

        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();

        StateHasChanged();
        addingDomRef!.StateHasChangedCall();
    }

    /// <inheritdoc/>
    protected override void RowEditPreviewHandler(object element)
        => elementBeforeEdit = GlobalTools.CreateDeepCopy((RowOfOrderDocumentModelDB)element);

    /// <inheritdoc/>
    protected override void RowEditCancelHandler(object element)
    {
        ((RowOfOrderDocumentModelDB)element).Quantity = elementBeforeEdit!.Quantity;
        elementBeforeEdit = null;
    }
}