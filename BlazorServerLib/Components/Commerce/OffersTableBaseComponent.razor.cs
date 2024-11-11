////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OffersTableBaseComponent
/// </summary>
public abstract class OffersTableBaseComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// CommerceRepo
    /// </summary>
    [Inject]
    protected ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


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


    /// <summary>
    /// allOffers
    /// </summary>    
    protected List<OfferGoodModelDB>? allOffers;

    /// <summary>
    /// editTrigger
    /// </summary>
    protected TableEditTrigger EditTrigger { get; set; } = TableEditTrigger.EditButton;


    /// <summary>
    /// Происходит, когда изменения фиксируются для редактируемой строки.
    /// </summary>
    protected void RowEditCommitHandler(object element)
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

    /// <summary>
    /// LoadOffers
    /// </summary>
    protected async Task LoadOffers(int page_num)
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
        await SetBusy(false);
        if (res.Success() && res.Response?.Response is not null && res.Response.Response.Count != 0)
        {
            allOffers!.AddRange(res.Response.Response);
            if (allOffers.Count < res.Response.TotalRowsCount)
                await LoadOffers(page_num + 1);
        }
    }

    /// <summary>
    /// AddingOfferAction
    /// </summary>
    protected abstract void AddingOfferAction(OfferGoodActionModel off);

    /// <summary>
    /// Происходит до начала редактирования строки.
    /// </summary>
    protected abstract void RowEditPreviewHandler(object element);

    /// <summary>
    /// Происходит, когда изменения отменяются для редактируемой строки.
    /// </summary>
    protected abstract void RowEditCancelHandler(object element);

    /// <summary>
    /// DeleteRow
    /// </summary>
    protected abstract void DeleteRow(int offerId);
}