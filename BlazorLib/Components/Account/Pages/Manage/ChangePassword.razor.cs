using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.Account.Pages.Manage;

/// <summary>
/// 
/// </summary>
public partial class ChangePassword : ComponentBase
{
    [Inject]
    IdentityRedirectManager RedirectManager { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;

    [SupplyParameterFromForm]
    private ChangePasswordModel Input { get; set; } = new();

    IEnumerable<ResultMessage>? Messages;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        UserBooleanResponseModel rest = await UsersProfilesRepo.UserHasPasswordAsync();
        //user = rest.UserInfo;
        Messages = rest.Messages;
        if (rest.Response != true)
        {
            RedirectManager.RedirectTo("Account/Manage/SetPassword");
        }
    }

    private async Task OnValidSubmitAsync()
    {
        Messages = null;
        ResponseBaseModel changePasswordResult = await UsersProfilesRepo.ChangePasswordAsync(Input.OldPassword, Input.NewPassword);
        Messages = changePasswordResult.Messages;
        if (!changePasswordResult.Success())
            return;

        Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = "Ваш пароль был изменен" }];
    }
}