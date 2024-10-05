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

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Card title
    /// </summary>
    [Parameter, EditorRequired]
    public required string Title { get; set; }

    /// <summary>
    /// KeyStorage
    /// </summary>
    [Parameter, EditorRequired]
    public required StorageCloudParameterModel KeyStorage { get; set; }

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

    long initValue;

    double heightCard;
    void SetHeightCard(double set_h)
    {
        heightCard = set_h;
        StateHasChanged();
    }

    async void SelectChatHandler(ChatTelegramModelDB? selected)
    {
        if (SelectedChat?.ChatTelegramId == selected?.ChatTelegramId || SelectedChat is null)
        {
            SelectedChat = selected;
            StateHasChanged();

            if (ChatChangeHandler is not null)
                ChatChangeHandler(this);

            return;
        }
        SelectedChat = selected;

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<int> rest = await StorageRepo.SaveParameter(selected?.ChatTelegramId, KeyStorage, false);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        StateHasChanged();

        if (ChatChangeHandler is not null)
            ChatChangeHandler(this);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<long?> rest = await StorageRepo.ReadParameter<long?>(KeyStorage);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        initValue = rest.Response ?? 0;
    }
}