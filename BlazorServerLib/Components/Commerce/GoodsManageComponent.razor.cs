////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// GoodsManageComponent
/// </summary>
public partial class GoodsManageComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    bool _expanded;
    MudTable<GoodsModelDB> tableRef = default!;

    async void CreateGoodsAction(GoodsModelDB goods)
    {
        await tableRef.ReloadServerData();
        OnExpandCollapseClick();
        StateHasChanged();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<GoodsModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<GoodsSelectRequestModel> req = new()
        {
            Payload = new(),
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<GoodsModelDB>?> res = await CommerceRepo.GoodsSelect(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (!res.Success() || res.Response?.Response is null)
            return new TableData<GoodsModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<GoodsModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }

    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }
}