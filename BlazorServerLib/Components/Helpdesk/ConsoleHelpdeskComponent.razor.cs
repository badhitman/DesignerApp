////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// ConsoleHelpdeskComponent
/// </summary>
public partial class ConsoleHelpdeskComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageRepo { get; set; } = default!;


    readonly List<StatusesDocumentsEnum> Steps = [.. Enum.GetValues(typeof(StatusesDocumentsEnum)).Cast<StatusesDocumentsEnum>()];
    byte stepNum;
    bool IsLarge;
    string? FilterUserId;


    async void SelectUserHandler(UserInfoModel? selected)
    {
        if (string.IsNullOrWhiteSpace(FilterUserId) && string.IsNullOrWhiteSpace(selected?.UserId) || FilterUserId == selected?.UserId)
            return;

        FilterUserId = selected?.UserId;
        stepNum = 0;

        await SetBusy();
        TResponseModel<int> res = await StorageRepo.SaveParameter(FilterUserId, GlobalStaticConstants.CloudStorageMetadata.ConsoleFilterForUser(CurrentUserSession!.UserId), false);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        StateHasChanged();
    }

    StorageMetadataModel SizeColumnsKeyStorage => new()
    {
        ApplicationName = Path.Combine(GlobalStaticConstants.Routes.CONSOLE_CONTROLLER_NAME, GlobalStaticConstants.Routes.HELPDESK_CONTROLLER_NAME),
        PropertyName = GlobalStaticConstants.Routes.SIZE_CONTROLLER_NAME,
        PrefixPropertyName = CurrentUserSession!.UserId,
    };

    async Task ToggleSize()
    {
        IsLarge = !IsLarge;
        stepNum = 0;

        await SetBusy();

        TResponseModel<int> res = await StorageRepo.SaveParameter(IsLarge, SizeColumnsKeyStorage, true);
        IsBusyProgress = false;
        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        await ReadCurrentUser();

        TResponseModel<bool> res = await StorageRepo.ReadParameter<bool>(SizeColumnsKeyStorage);
        TResponseModel<string?> current_filter_user_res = await StorageRepo.ReadParameter<string>(GlobalStaticConstants.CloudStorageMetadata.ConsoleFilterForUser(CurrentUserSession!.UserId));
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(res.Messages);
        SnackbarRepo.ShowMessagesResponse(current_filter_user_res.Messages);

        FilterUserId = current_filter_user_res.Response;
        IsLarge = res.Response == true;
    }
}