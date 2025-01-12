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
    ITelegramTransmission TelegramRepo { get; set; } = default!;


    /// <summary>
    /// Selected chat
    /// </summary>
    [Parameter, EditorRequired]
    public required long SelectedChat { get; set; }


    /// <inheritdoc/>
    public override async Task LoadPartData()
    {
        await SetBusy();
        TPaginationResponseModel<ChatTelegramModelDB> rest = await TelegramRepo
            .ChatsSelect(new()
            {
                Payload = _selectedValueText,
                PageNum = PageNum,
                PageSize = page_size,
            });

        if (rest.Response is not null)
        {
            TotalRowsCount = rest.TotalRowsCount;
            LoadedData.AddRange(rest.Response);

            if (PageNum == 0)
                LoadedData.Insert(0, new() { Title = "OFF" });

            PageNum++;
        }
        await SetBusy(false);
    }

    long _sc;
    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender && _sc != SelectedChat)
        {
            _sc = SelectedChat;
            await ReadChat();
        }
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
        await ReadChat();
    }

    async Task ReadChat()
    {
        await SetBusy();

        List<ChatTelegramModelDB> rest = await TelegramRepo.ChatsReadTelegram([SelectedChat]);
        
        if (rest.Count == 0)
        {
            await SetBusy(false);
            SnackbarRepo.Error($"Не найден запрашиваемый чат #{SelectedChat}");
            return;
        }
        SelectedObject = rest.Single();
        _selectedValueText = SelectedObject.ToString();
        SelectHandleAction(SelectedObject);
        await SetBusy(false);
    }
}