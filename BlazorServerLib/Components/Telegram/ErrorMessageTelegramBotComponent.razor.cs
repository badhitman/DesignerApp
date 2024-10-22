////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// ErrorMessageTelegramBotComponent
/// </summary>
public partial class ErrorMessageTelegramBotComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;


    /// <summary>
    /// ChatId
    /// </summary>
    [Parameter]
    public long? ChatId { get; set; }


    private IEnumerable<ErrorSendingMessageTelegramBotModelDB> pagedData = default!;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<ErrorSendingMessageTelegramBotModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        await SetBusy();
        TResponseModel<TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>?> err_res = await TelegramRepo.ErrorsForChatsSelectTelegram(new TPaginationRequestModel<long[]?>()
        {
            Payload = ChatId is null || ChatId.Value == 0 ? null : [ChatId.Value],
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(err_res.Messages);
        if (!err_res.Success() || err_res.Response?.Response is null)
            return new TableData<ErrorSendingMessageTelegramBotModelDB>() { TotalItems = 0, Items = [] };

        pagedData = err_res.Response.Response;
        return new TableData<ErrorSendingMessageTelegramBotModelDB>() { TotalItems = err_res.Response.TotalRowsCount, Items = pagedData };
    }
}