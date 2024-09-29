////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;
using static SharedLib.GlobalStaticConstants;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// ConsoleHelpdeskComponent
/// </summary>
public partial class ConsoleHelpdeskComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService storageRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;


    readonly List<HelpdeskIssueStepsEnum> Steps = [.. Enum.GetValues(typeof(HelpdeskIssueStepsEnum)).Cast<HelpdeskIssueStepsEnum>()];
    byte stepNum;
    bool IsLarge;
    string? FilterUserId;

    UserInfoMainModel CurrentUser = default!;

    async void SelectUserHandler(UserInfoModel? selected)
    {
        if (string.IsNullOrWhiteSpace(FilterUserId) && string.IsNullOrWhiteSpace(selected?.UserId) || FilterUserId == selected?.UserId)
            return;

        FilterUserId = selected?.UserId;
        stepNum = 0;

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<int> res = await storageRepo.SaveParameter(FilterUserId, CloudStorageMetadata.ConsoleFilterForUser(CurrentUser.UserId));
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        StateHasChanged();
    }

    StorageCloudParameterModel KeyStorageSizeColumns => new()
    {
        ApplicationName = Path.Combine(Routes.CONSOLE_CONTROLLER_NAME, Routes.HELPDESK_CONTROLLER_NAME),
        Name = Routes.SIZE_CONTROLLER_NAME,
        PrefixPropertyName = CurrentUser.UserId,
    };

    async Task ToggleSize()
    {
        IsLarge = !IsLarge;
        stepNum = 0;

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<int> res = await storageRepo.SaveParameter(IsLarge, KeyStorageSizeColumns);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        CurrentUser = state.User.ReadCurrentUserInfo() ?? throw new Exception();

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<bool> res = await storageRepo.ReadParameter<bool>(KeyStorageSizeColumns);
        TResponseModel<string?> current_filter_user_res = await storageRepo.ReadParameter<string>(CloudStorageMetadata.ConsoleFilterForUser(CurrentUser.UserId));
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(res.Messages);
        SnackbarRepo.ShowMessagesResponse(current_filter_user_res.Messages);

        FilterUserId = current_filter_user_res.Response;
        IsLarge = res.Response == true;
    }
}