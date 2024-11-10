////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using BlazorWebLib.Components.Commerce;

namespace BlazorWebLib.Components.Warehouse;

/// <summary>
/// WarehouseEditingComponent
/// </summary>
public partial class WarehouseEditingComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Id
    /// </summary>
    [Parameter, EditorRequired]
    public required int Id { get; set; }

    AddRowToOrderDocumentComponent? addingDomRef;

    void AddingOfferAction(OfferGoodActionModel off)
    {
        //CurrentTab.Rows ??= [];
        //int exist_row = CurrentTab.Rows.FindIndex(x => x.OfferId == off.Id);
        //if (exist_row < 0)
        //    CurrentTab.Rows.Add(new RowOfOrderDocumentModelDB()
        //    {
        //        AddressForOrderTab = CurrentTab,
        //        AddressForOrderTabId = CurrentTab.Id,
        //        Goods = off.Goods,
        //        GoodsId = off.GoodsId,
        //        Offer = off,
        //        OfferId = off.Id,
        //        OrderDocument = CurrentTab.OrderDocument,
        //        OrderDocumentId = CurrentTab.OrderDocumentId,
        //        Quantity = off.Quantity,
        //    });
        //else
        //    CurrentTab.Rows[exist_row].Quantity = +off.Quantity;

        //if (DocumentUpdateHandler is not null)
        //    DocumentUpdateHandler();

        //StateHasChanged();
        //addingDomRef!.StateHasChangedCall();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server, with a token for canceling this request
    /// </summary>
    private async Task<TableData<RowOfWarehouseDocumentModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        // Return the data
        return new TableData<RowOfWarehouseDocumentModelDB>() { TotalItems = 0, Items = [] };
    }
}