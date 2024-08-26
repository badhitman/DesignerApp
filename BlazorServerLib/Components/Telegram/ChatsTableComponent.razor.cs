////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

public partial class ChatsTableComponent : BlazorBusyComponentBaseModel
{
    private IEnumerable<ChatTelegramModelDB> pagedData = [];
    private MudTable<ChatTelegramModelDB> table = default!;

    private int totalItems;
    private string? searchString = null;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<ChatTelegramModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        
        return new TableData<ChatTelegramModelDB>() { TotalItems = totalItems, Items = pagedData };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
}