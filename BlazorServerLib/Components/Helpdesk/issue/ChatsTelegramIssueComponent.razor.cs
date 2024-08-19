////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// ChatsTelegramIssueComponent
/// </summary>
public partial class ChatsTelegramIssueComponent : IssueWrapBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService tgRepo { get; set; } = default!;

    ChatTelegramModelDB[]? chats = null;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        long[] chats_ids = [.. UsersIdentityDump.Where(x => x.TelegramId.HasValue).Select(x => x.TelegramId!.Value)];
        IsBusyProgress = true;
        TResponseModel<ChatTelegramModelDB[]?> rest_chats = await tgRepo.ChatsReadTelegram(chats_ids);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest_chats.Messages);
        chats = rest_chats.Response;
    }
}