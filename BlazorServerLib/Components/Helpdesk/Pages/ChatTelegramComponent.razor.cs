////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
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

    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;


    /// <summary>
    /// Chat id (db:id)
    /// </summary>
    [Parameter]
    public int? ChatId { get; set; }


    HelpdeskJournalComponent? _tab;

    void Update()
    {
        _tab?.TableRef.ReloadServerData();
    }

    void SetTab(HelpdeskJournalComponent page)
    {
        _tab = page;
    }

    ChatTelegramModelDB? Chat;
    TelegramUserBaseModel? CurrentUser;

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
        if (Chat is null)
            return;

        IsBusyProgress = true;
        TResponseModel<TelegramUserBaseModel?> get_user = await WebRepo.GetTelegramUser(Chat.ChatTelegramId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(get_user.Messages);
        CurrentUser = get_user.Response;
    }
}