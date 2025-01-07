////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// NotificationsAreaTelegramIssueConfigComponent
/// </summary>
public partial class NotificationsAreaTelegramIssueConfigComponent : IssueWrapBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;


    List<ChatTelegramModelDB>? chatsTelegram;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        long[] telegram_users_ids = UsersIdentityDump
            .Where(x => x.TelegramId.HasValue)
            .Select(x => x.TelegramId!.Value)
            .ToArray();

        if (telegram_users_ids.Length == 0)
            return;

        await SetBusy();
        chatsTelegram = [.. await TelegramRepo.ChatsFindForUser(telegram_users_ids)];
        IsBusyProgress = false;
        chatsTelegram.Insert(0, new() { Title = "Off", Type = ChatsTypesTelegramEnum.Private });
    }
}