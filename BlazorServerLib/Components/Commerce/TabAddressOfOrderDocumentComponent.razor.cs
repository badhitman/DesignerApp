////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// TabAddressOfOrderDocumentComponent
/// </summary>
public partial class TabAddressOfOrderDocumentComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// CurrentTab
    /// </summary>
    [Parameter, EditorRequired]
    public required TabAddressForOrderModelDb CurrentTab { get; set; }

    /// <summary>
    /// ReadOnly
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// DocumentUpdateHandler
    /// </summary>
    [Parameter]
    public Action? DocumentUpdateHandler { get; set; }


    TableEditTrigger editTrigger = TableEditTrigger.EditButton;
    List<OfferGoodModelDB>? allOffers;
    AddRowToOrderDocumentComponent? addingDomRef;
    RowOfOrderDocumentModelDB? elementBeforeEdit;

        
    void DeleteRow(RowOfOrderDocumentModelDB row)
    {
        CurrentTab.Rows ??= [];
        CurrentTab.Rows.RemoveAll(x => x.OfferId == row.OfferId);
        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();

        addingDomRef?.StateHasChangedCall();
    }

    void AddingOfferAction(OfferGoodActionModel off)
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

    /// <summary>
    /// Происходит до начала редактирования строки.
    /// </summary>
    void RowEditPreviewHandler(object element)
        => elementBeforeEdit = GlobalTools.CreateDeepCopy((RowOfOrderDocumentModelDB)element);

    /// <summary>
    /// Происходит, когда изменения отменяются для редактируемой строки.
    /// </summary>
    private void RowEditCancelHandler(object element)
    {
        ((RowOfOrderDocumentModelDB)element).Quantity = elementBeforeEdit!.Quantity;
        elementBeforeEdit = null;
    }

    /// <summary>
    /// Происходит, когда изменения фиксируются для редактируемой строки.
    /// </summary>
    void RowEditCommitHandler(object element)
    {
        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (!ReadOnly)
            await LoadOffers(0);
    }

    async Task LoadOffers(int page_num)
    {
        if (page_num == 0)
            allOffers = [];

        TPaginationRequestModel<OffersSelectRequestModel> req = new()
        {
            PageNum = page_num,
            PageSize = 10,
            SortBy = nameof(OfferGoodModelDB.Name),
            SortingDirection = VerticalDirectionsEnum.Up,
            Payload = new()
        };
        await SetBusy();
        
        TResponseModel<TPaginationResponseModel<OfferGoodModelDB>> res = await CommerceRepo.OffersSelect(req);
        IsBusyProgress = false;
        if (res.Success() && res.Response?.Response is not null && res.Response.Response.Count != 0)
        {
            allOffers!.AddRange(res.Response.Response);
            if (allOffers.Count < res.Response.TotalRowsCount)
                await LoadOffers(page_num + 1);
        }
    }
}