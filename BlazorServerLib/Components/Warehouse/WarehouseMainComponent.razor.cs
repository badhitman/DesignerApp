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
public partial class WarehouseMainComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// CommRepo
    /// </summary>
    [Inject]
    ICommerceRemoteTransmissionService CommRepo { get; set; } = default!;

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
        TResponseModel<TPaginationResponseModel<WarehouseDocumentModelDB>> rest = await CommRepo.WarehousesSelect(req);
        await SetBusy(false, token: token);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (rest.Response is null)
            return new TableData<WarehouseDocumentModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<WarehouseDocumentModelDB>() { TotalItems = rest.Response.TotalRowsCount, Items = rest.Response.Response };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table?.ReloadServerData();
    }
}