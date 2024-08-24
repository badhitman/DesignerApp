////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// ChatSelectInputComponent
/// </summary>
public partial class ChatSelectInputComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Selected chat
    /// </summary>
    [Parameter, EditorRequired]
    public required long SelectedChat { get; set; }

    /// <summary>
    /// Поддержка NULL выбора в селекторе, указывающий на то что ни чего не выбрано
    /// </summary>
    [Parameter]
    public bool NullableAllow { get; set; }

    /// <summary>
    /// SelectChatHandle
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<ChatTelegramModelDB?> SelectChatHandle { get; set; }


    ChatTelegramModelDB selectedChatDb = default!;

    bool IsEditing;

    void EditToggle()
    {
        IsEditing = !IsEditing;
    }

    string? _selectedChatText;
    string? SelectedChatText
    {
        get => IsEditing ? _selectedChatText : selectedChatDb.ToString();
        set
        {
            _selectedChatText = value;
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (SelectedChat < 1)
        {
            selectedChatDb = new() { Title = "OFF", Type = ChatsTypesTelegramEnum.Private };
            SelectChatHandle(selectedChatDb);
            return;
        }

        IsBusyProgress = true;
        TResponseModel<ChatTelegramModelDB[]?> rest = await TelegramRepo.ChatsReadTelegram([SelectedChat]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Response is null || rest.Response.Length == 0)
        {
            SnackbarRepo.Error($"Не найден запрашиваемый чат #{SelectedChat}");
            return;
        }
        selectedChatDb = rest.Response.Single();
        _selectedChatText = selectedChatDb.ToString();
        SelectChatHandle(selectedChatDb);
    }
}