////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// Выбор Telegram чата для глобальной переадресации клиентов, которым не обнаружена подписка среди сотрудников
/// </summary>
public partial class ChatSelectComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageRepo { get; set; } = default!;


    /// <summary>
    /// Card title
    /// </summary>
    [Parameter, EditorRequired]
    public required string Title { get; set; }

    /// <summary>
    /// KeyStorage
    /// </summary>
    [Parameter, EditorRequired]
    public required StorageMetadataModel KeyStorage { get; set; }

    /// <summary>
    /// CArd subtitle
    /// </summary>
    [Parameter]
    public string? Subtitle { get; set; }

    /// <summary>
    /// Hint
    /// </summary>
    [Parameter]
    public string? Hint { get; set; }

    /// <summary>
    /// Выбор чата
    /// </summary>
    [CascadingParameter]
    public Action<ChatSelectComponent>? ChatChangeHandler { get; set; }


    /// <summary>
    /// Выбранный чат
    /// </summary>
    public ChatTelegramModelDB? SelectedChat;
    ChatSelectInputComponent? selectorInput;

    long initValue;

    double heightCard;
    void SetHeightCard(double set_h)
    {
        heightCard = set_h;
        StateHasChanged();
    }

    async void SelectChatHandler(ChatTelegramModelDB? selected)
    {
        if (SelectedChat?.ChatTelegramId == selected?.ChatTelegramId || SelectedChat is null || initValue == selected?.ChatTelegramId)
        {
            SelectedChat = selected;
            StateHasChanged();

            if (ChatChangeHandler is not null)
                ChatChangeHandler(this);

            return;
        }
        SelectedChat = selected;

        await SetBusy();
        TResponseModel<int> rest = await StorageRepo.SaveParameter(selected?.ChatTelegramId, KeyStorage, false);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        await SetBusy(false);

        if (ChatChangeHandler is not null)
            ChatChangeHandler(this);
    }

    long _sv = 0;
    /// <inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender && selectorInput is not null && _sv != initValue)
        {
            _sv = initValue;
            selectorInput.StateHasChangedCall();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        TResponseModel<long?> rest = await StorageRepo.ReadParameter<long?>(KeyStorage);
        if (!rest.Success())
            SnackbarRepo.ShowMessagesResponse(rest.Messages);
        initValue = rest.Response ?? 0;
        IsBusyProgress = false;
    }
}