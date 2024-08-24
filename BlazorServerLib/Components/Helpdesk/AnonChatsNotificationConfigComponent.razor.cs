////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// Выбор Telegram чата для глобальной переадресации клиентов, которым не обнаружена подписка среди сотрудников
/// </summary>
public partial class AnonChatsNotificationConfigComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    ChatTelegramModelDB? selectedChat;

    long initValue;

    static StorageCloudParameterModel KeyStorage => new()
    {
        ApplicationName = GlobalStaticConstants.Routes.HELPDESK_CONTROLLER_NAME,
        Name = $"{GlobalStaticConstants.Routes.TELEGRAM_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.NOTIFICATIONS_CONTROLLER_NAME}",
        PrefixPropertyName = GlobalStaticConstants.Routes.GLOBAL_CONTROLLER_NAME,
        OwnerPrimaryKey = 0,
    };

    double heightCard;
    void setHeightCard(double set_h)
    {
        heightCard = set_h;
        StateHasChanged();
    }

    async void SelectChatHandler(ChatTelegramModelDB? selected)
    {
        selectedChat = selected;
        if (initValue == selected?.Id)
        {
            StateHasChanged();
            return;
        }

        IsBusyProgress = true;
        TResponseModel<int> rest = await StorageRepo.SaveParameter(selected?.ChatTelegramId, KeyStorage);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        StateHasChanged();
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