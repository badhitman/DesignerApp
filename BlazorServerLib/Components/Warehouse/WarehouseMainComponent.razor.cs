////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Warehouse;

/// <summary>
/// WarehouseMainComponent
/// </summary>
public partial class WarehouseMainComponent : BlazorBusyComponentBaseModel
{
    private MudTable<WarehouseDocumentModelDB>? table;

    private int totalItems;
    private string? searchString = null;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<WarehouseDocumentModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        
        return new TableData<WarehouseDocumentModelDB>() { TotalItems = totalItems, Items = [] };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table?.ReloadServerData();
    }
}