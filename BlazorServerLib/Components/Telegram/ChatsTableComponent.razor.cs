////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// ChatsTableComponent
/// </summary>
public partial class ChatsTableComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService tgRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackBarRepo { get; set; } = default!;


    private IEnumerable<ChatTelegramModelDB> pagedData = [];
    private MudTable<ChatTelegramModelDB> table = default!;

    private string? searchString = null;

    async Task<TableData<ChatTelegramModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        IsBusyProgress = true;
        TPaginationRequestModel<string?> req = new()
        {
            Payload = searchString,
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        TResponseModel<TPaginationResponseModel<ChatTelegramModelDB>?> rest = await tgRepo.ChatsSelect(req);
        IsBusyProgress = false;
        SnackBarRepo.ShowMessagesResponse(rest.Messages);

        if(!rest.Success() || rest.Response?.Response is null)
            return new TableData<ChatTelegramModelDB>() { TotalItems = 0, Items = pagedData };

        pagedData = rest.Response.Response;
        return new TableData<ChatTelegramModelDB>() { TotalItems = rest.Response.TotalRowsCount, Items = pagedData };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
}