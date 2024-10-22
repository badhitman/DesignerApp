////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// ChatSelectInputComponent
/// </summary>
public partial class ChatSelectInputComponent : LazySelectorComponent<ChatTelegramModelDB>
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;


    /// <summary>
    /// Selected chat
    /// </summary>
    [Parameter, EditorRequired]
    public required long SelectedChat { get; set; }


    /// <inheritdoc/>
    public override async Task LoadPartData()
    {
        await SetBusy();
        
        TResponseModel<TPaginationResponseModel<ChatTelegramModelDB>?> rest = await TelegramRepo
            .ChatsSelect(new()
            {
                Payload = _selectedValueText,
                PageNum = PageNum,
                PageSize = page_size,
            });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success() && rest.Response?.Response is not null)
        {
            TotalRowsCount = rest.Response.TotalRowsCount;
            LoadedData.AddRange(rest.Response.Response);

            if (PageNum == 0)
                LoadedData.Insert(0, new() { Title = "OFF" });

            PageNum++;
        }
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (SelectedChat == 0)
        {
            SelectedObject = new() { Title = "OFF", Type = ChatsTypesTelegramEnum.Private };
            SelectHandleAction(SelectedObject);
            return;
        }

        await SetBusy();
        
        TResponseModel<ChatTelegramModelDB[]?> rest = await TelegramRepo.ChatsReadTelegram([SelectedChat]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Response is null || rest.Response.Length == 0)
        {
            SnackbarRepo.Error($"Не найден запрашиваемый чат #{SelectedChat}");
            return;
        }
        SelectedObject = rest.Response.Single();
        _selectedValueText = SelectedObject.ToString();
        SelectHandleAction(SelectedObject);
    }
}