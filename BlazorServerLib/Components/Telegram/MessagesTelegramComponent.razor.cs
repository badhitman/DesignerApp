////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// MessagesTelegramComponent
/// </summary>
public partial class MessagesTelegramComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Id - of database
    /// </summary>
    [Parameter, EditorRequired]
    public int ChatId { get; set; }


    private string _searchStringQuery = "";
    private string searchStringQuery
    {
        get => _searchStringQuery;
        set
        {
            _searchStringQuery = value;
            InvokeAsync(TableRef.ReloadServerData);
        }
    }

    /// <summary>
    /// Table
    /// </summary>
    public MudTable<MessageTelegramModelDB> TableRef { get; set; } = default!;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<MessageTelegramModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<MessageTelegramModelDB>?> rest_message = await TelegramRepo
            .MessagesListTelegram(new TPaginationRequestModel<SearchMessagesChatModel>()
            {
                Payload = new() { ChatId = ChatId, SearchQuery = searchStringQuery },
                PageNum = state.Page,
                PageSize = state.PageSize,
                SortingDirection = state.SortDirection == SortDirection.Descending ? VerticalDirectionsEnum.Down : VerticalDirectionsEnum.Up,
                SortBy = state.SortLabel,
            });

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest_message.Messages);

        if (rest_message.Response is null)
            return new TableData<MessageTelegramModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<MessageTelegramModelDB>() { TotalItems = rest_message.Response.TotalRowsCount, Items = rest_message.Response.Response };
    }
}