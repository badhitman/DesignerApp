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
        TPaginationResponseModel<WarehouseDocumentModelDB> rest = await CommerceRepo.WarehousesSelect(req);
        await SetBusy(false, token: token);

        if (rest.Response is not null)
        {
            await CacheRubricsUpdate(rest.Response.Select(x => x.WarehouseId));
            return new TableData<WarehouseDocumentModelDB>() { TotalItems = rest.TotalRowsCount, Items = rest.Response };
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