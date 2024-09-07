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

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// CurrentTab
    /// </summary>
    [Parameter, EditorRequired]
    public required AddressForOrderModelDb CurrentTab { get; set; }

    /// <summary>
    /// ReadOnly
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }


    TableEditTrigger editTrigger = TableEditTrigger.EditButton;
    List<OfferGoodModelDB>? allOffers;


    void AddingOfferAction(OfferGoodActionModel off)
    {
        CurrentTab.Rows?.Add(new RowOfOrderDocumentModelDB()
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
        StateHasChanged();
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
            {
                GoodFilter = 0,
            }
        };
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<OfferGoodModelDB>?> res = await CommerceRepo.OffersSelect(req);
        IsBusyProgress = false;
        if (res.Success() && res.Response?.Response is not null && res.Response.Response.Count != 0)
        {
            allOffers!.AddRange(res.Response.Response);
            if (allOffers.Count < res.Response.TotalRowsCount)
                await LoadOffers(page_num + 1);
        }
    }

    private void BackupItem(object element)
    {
        //elementBeforeEdit = new()
        //{
        //    Sign = ((Element)element).Sign,
        //    Name = ((Element)element).Name,
        //    Molar = ((Element)element).Molar,
        //    Position = ((Element)element).Position
        //};
        //AddEditionEvent($"RowEditPreview event: made a backup of Element {((Element)element).Name}");
    }

    private void ResetItemToOriginalValues(object element)
    {
        //((Element)element).Sign = elementBeforeEdit.Sign;
        //((Element)element).Name = elementBeforeEdit.Name;
        //((Element)element).Molar = elementBeforeEdit.Molar;
        //((Element)element).Position = elementBeforeEdit.Position;
        //AddEditionEvent($"RowEditCancel event: Editing of Element {((Element)element).Name} canceled");
    }

    void ItemHasBeenCommitted(object element)
    {

    }
    void ItemHasBeenOnCommitEditClick()
    {

    }
}