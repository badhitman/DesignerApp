////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.Pages;

/// <summary>
/// ChatTelegramComponent
/// </summary>
public partial class ChatTelegramComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Chat id (db:id)
    /// </summary>
    [Parameter]
    public int? ChatId { get; set; }


    ChatTelegramModelDB? Chat;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (ChatId is null || ChatId < 1)
            return;

        IsBusyProgress = true;
        TResponseModel<ChatTelegramModelDB?> rest = await TelegramRepo.ChatTelegramRead(ChatId.Value);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);        
        Chat = rest.Response;
    }
}