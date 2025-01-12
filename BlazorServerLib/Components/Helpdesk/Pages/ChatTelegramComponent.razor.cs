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
    ITelegramTransmission TelegramRepo { get; set; } = default!;

    [Inject]
    IWebTransmission WebRepo { get; set; } = default!;


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

        await SetBusy();
        Chat = await TelegramRepo.ChatTelegramRead(ChatId.Value);
        IsBusyProgress = false;
        await SetBusy();
        TResponseModel<TelegramUserBaseModel> get_user = await WebRepo.GetTelegramUser(Chat.ChatTelegramId);
        IsBusyProgress = false;
        //SnackbarRepo.ShowMessagesResponse(get_user.Messages);
        CurrentUser = get_user.Response;
    }
}