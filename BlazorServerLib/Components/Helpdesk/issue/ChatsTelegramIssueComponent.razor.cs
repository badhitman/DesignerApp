////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// ChatsTelegramIssueComponent
/// </summary>
public partial class ChatsTelegramIssueComponent : IssueWrapBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;


    ChatTelegramModelDB[]? chats = null;

    async void SendMessageTelegramAction(SendTextMessageTelegramBotModel msg)
    {
        PulseRequestModel req_pulse = new()
        {
            Payload = new()
            {
                Payload = new()
                {
                    Description = $"Отправил сообщение в Telegram: user-tg#{msg.UserTelegramId}",
                    IssueId = Issue.Id,
                    PulseType = PulseIssuesTypesEnum.Messages,
                    Tag = GlobalStaticConstants.Routes.TELEGRAM_CONTROLLER_NAME,
                },
                SenderActionUserId = CurrentUserSession!.UserId
            }
        };
        await SetBusy();
        await HelpdeskRepo.PulsePush(req_pulse, false);
        TResponseModel<int> add_msg_system = await HelpdeskRepo.MessageCreateOrUpdate(new()
        {
            SenderActionUserId = GlobalStaticConstants.Roles.System,
            Payload = new() { MessageText = $"<b>Пользователь {CurrentUserSession!.UserName} отправил сообщение Telegram пользователю user-tg#{msg.UserTelegramId}</b>: {msg.Message}", IssueId = Issue.Id }
        });
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(add_msg_system.Messages);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        await base.OnInitializedAsync();
        long[] chats_ids = [.. UsersIdentityDump.Where(x => x.TelegramId.HasValue).Select(x => x.TelegramId!.Value)];

        TResponseModel<ChatTelegramModelDB[]?> rest_chats = await TelegramRepo.ChatsReadTelegram(chats_ids);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest_chats.Messages);
        chats = rest_chats.Response;
    }
}