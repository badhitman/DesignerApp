////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// TelegramChatWrapComponent
/// </summary>
public partial class TelegramChatWrapComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Чат
    /// </summary>
    [Parameter, EditorRequired]
    public required ChatTelegramModelDB Chat { get; set; }

    string? _textSendMessage;

    MessagesTelegramComponent _messagesTelegramComponent = default!;

    bool NavbarToggle = true;

    string GetTitle()
    {
        if (!string.IsNullOrWhiteSpace(Chat.Username))
            return $"@{Chat.Username}";

        if (!string.IsNullOrWhiteSpace(Chat.Title))
            return $"{Chat.Title} /{Chat.FirstName} {Chat.LastName}";

        if (!string.IsNullOrWhiteSpace(Chat.FirstName) || !string.IsNullOrWhiteSpace(Chat.LastName))
            return $"{Chat.FirstName} {Chat.LastName}";

        return $"#{Chat.ChatTelegramId}";
    }
}