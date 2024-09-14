////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OffersOfGoodsComponent
/// </summary>
public partial class OffersOfGoodsComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// CurrentGoods
    /// </summary>
    [Parameter, EditorRequired]
    public required GoodsModelDB CurrentGoods { get; set; }


    private MudTable<OfferGoodModelDB> table = default!;


    async void CreateOfferAction(OfferGoodModelDB sender)
    {
        await table.ReloadServerData();
        OnExpandCollapseClick();
        StateHasChanged();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OfferGoodModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<OffersSelectRequestModel> req = new()
        {
            Payload = new()
            {
                GoodFilter = CurrentGoods.Id
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<OfferGoodModelDB>> res = await CommerceRepo.OffersSelect(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (!res.Success() || res.Response?.Response is null)
            return new TableData<OfferGoodModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<OfferGoodModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }

    bool _expanded;
    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }
}