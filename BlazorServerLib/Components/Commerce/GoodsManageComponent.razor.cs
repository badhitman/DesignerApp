////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// GoodsManageComponent
/// </summary>
public partial class GoodsManageComponent : BlazorBusyComponentRegistersModel
{
    [Inject]
    IJSRuntime JsRuntimeRepo { get; set; } = default!;


    bool _expanded;
    MudTable<GoodsModelDB> tableRef = default!;


    async void CreateGoodsAction(GoodsModelDB goods)
    {
        await tableRef.ReloadServerData();
        OnExpandCollapseClick();
        StateHasChanged();
    }

    async Task DownloadFullPrice()
    {
        await SetBusy();
        TResponseModel<FileAttachModel> res = await CommerceRepo.PriceFullFileGet();
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await SetBusy(false);
        if (res.Success() && res.Response is not null && res.Response.Data.Length != 0)
        {
            using MemoryStream ms = new(res.Response.Data);
            using DotNetStreamReference streamRef = new(stream: ms);
            await JsRuntimeRepo.InvokeVoidAsync("downloadFileFromStream", res.Response.Name, streamRef);
        }
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
        await SetBusy(token: token);
        TResponseModel<TPaginationResponseModel<GoodsModelDB>> res = await CommerceRepo.GoodsSelect(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (res.Success() && res.Response?.Response is not null)
        {
            await CacheRegistersGoodsUpdate(res.Response.Response.Select(x => x.Id));
            IsBusyProgress = false;
            return new TableData<GoodsModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
        }

        IsBusyProgress = false;

        if (!res.Success() || res.Response?.Response is null)
            return new TableData<GoodsModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<GoodsModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }

    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }
}