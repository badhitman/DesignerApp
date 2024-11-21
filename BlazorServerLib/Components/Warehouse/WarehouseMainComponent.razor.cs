////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Warehouse;

/// <summary>
/// WarehouseMainComponent
/// </summary>
public partial class WarehouseMainComponent : BlazorBusyComponentRubricsCachedModel
{
    private MudTable<WarehouseDocumentModelDB>? table;

    private string? searchString = null;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<WarehouseDocumentModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        await SetBusy(token: token);
        TPaginationRequestModel<WarehouseDocumentsSelectRequestModel> req = new()
        {
            Payload = new()
            {
                SearchQuery = searchString,
                IncludeExternalData = true,
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        TResponseModel<TPaginationResponseModel<WarehouseDocumentModelDB>> rest = await CommerceRepo.WarehousesSelect(req);
        await SetBusy(false, token: token);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (rest.Response is not null)
        {
            await CacheRubricsUpdate(rest.Response.Response.Select(x => x.WarehouseId));
            return new TableData<WarehouseDocumentModelDB>() { TotalItems = rest.Response.TotalRowsCount, Items = rest.Response.Response };
        }

        await SetBusy(false, token: token);
        return new TableData<WarehouseDocumentModelDB>() { TotalItems = 0, Items = [] };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table?.ReloadServerData();
    }
}