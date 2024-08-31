////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

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
    UserInfoMainModel CurrentUser = default!;

    async void SelectUserHandler(UserInfoModel? selected)
    {

    }

    StorageCloudParameterModel KeyStorage => new()
    {
        ApplicationName = GlobalStaticConstants.Routes.CONSOLE_CONTROLLER_NAME,
        Name = GlobalStaticConstants.Routes.SIZE_CONTROLLER_NAME,
        PrefixPropertyName = CurrentUser.UserId,
    };

    async Task ToggleSize()
    {
        IsLarge = !IsLarge;
        stepNum = 0;

        IsBusyProgress = true;
        TResponseModel<int> res = await storageRepo.SaveParameter(IsLarge, KeyStorage);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        CurrentUser = state.User.ReadCurrentUserInfo() ?? throw new Exception();

        IsBusyProgress = true;
        TResponseModel<bool> res = await storageRepo.ReadParameter<bool>(KeyStorage);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        IsLarge = res.Response == true;
    }
}