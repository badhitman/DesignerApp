////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// ChatsWappiIssueComponent
/// </summary>
public partial class ChatsWappiIssueComponent : IssueWrapBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;


    string[]? chats = null;

    async void SendMessageWhatsAppAction(EntryAltExtModel msg)
    {
        PulseRequestModel req_pulse = new()
        {
            Payload = new()
            {
                Payload = new()
                {
                    Description = $"Отправил сообщение в WhatsApp: {msg.Number}<hr/>{msg.Text}",
                    IssueId = Issue.Id,
                    PulseType = PulseIssuesTypesEnum.Messages,
                    Tag = GlobalStaticConstants.Routes.WAPPI_CONTROLLER_NAME,
                },
                SenderActionUserId = CurrentUserSession!.UserId
            }
        };
        await SetBusy();
        TResponseModel<int> add_msg_system = default!;
        List<Task> tasks = [HelpdeskRepo.PulsePush(req_pulse, false),
        Task.Run(async () => { 
            add_msg_system = await HelpdeskRepo.MessageCreateOrUpdate(new() { SenderActionUserId = GlobalStaticConstants.Roles.System, Payload = new() { MessageText = $"<b>Пользователь {CurrentUserSession!.UserName} отправил сообщение WhatsApp пользователю {msg.Number}</b>: {msg.Text}", IssueId = Issue.Id }});
        })];
        await Task.WhenAll(tasks);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(add_msg_system.Messages);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        chats = [.. UsersIdentityDump.Where(x => GlobalTools.IsPhoneNumber(x.PhoneNumber)).Select(x => x.PhoneNumber)];
    }
}