////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// WappiChatWrapComponent
/// </summary>
public partial class WappiChatWrapComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;


    /// <summary>
    /// Чат
    /// </summary>
    [Parameter, EditorRequired]
    public required string Chat { get; set; }

    /// <summary>
    /// SendMessageHandle
    /// </summary>
    [CascadingParameter]
    public Action<EntryAltExtModel>? SendMessageHandle { get; set; }


    string? _textSendMessage;

    bool NavbarToggle = true;

    async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(_textSendMessage))
            throw new ArgumentNullException(nameof(_textSendMessage));

        await SetBusy();
        EntryAltExtModel req = new() { Text = _textSendMessage, Number = Chat };

        TResponseModel<SendMessageResponseModel?> rest = await TelegramRepo.SendWappiMessage(req);
        _textSendMessage = "";

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        
        if (SendMessageHandle is not null)
            SendMessageHandle(req);
    }
}