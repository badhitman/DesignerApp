////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using BlazorWebLib.Components.Helpdesk.issue;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// NotificationsAreaTelegramIssueConfigComponent
/// </summary>
public partial class NotificationsAreaTelegramIssueConfigComponent : IssueWrapBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;


    List<ChatTelegramModelDB>? chatsTelegram;

    const string AppName = $"{GlobalStaticConstants.Routes.HELPDESK_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.NOTIFICATIONS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TELEGRAM_CONTROLLER_NAME}";

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        long[] telegram_users_ids = UsersIdentityDump
            .Where(x => x.TelegramId.HasValue)
            .Select(x => x.TelegramId!.Value)
            .ToArray();

        if (telegram_users_ids.Length == 0)
            return;

        IsBusyProgress = true;
        TResponseModel<ChatTelegramModelDB[]?> rest = await TelegramRepo.ChatsFindForUser(telegram_users_ids);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        chatsTelegram = [.. rest.Response];
        chatsTelegram.Insert(0, new() { Title = "Off", Type = ChatsTypesTelegramEnum.Private });
    }
}