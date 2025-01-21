////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Account.Pages.Manage;

/// <summary>
/// ChangePasswordPage
/// </summary>
public partial class ChangePasswordPage
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
        TResponseModel<bool?> rest = await UsersProfilesRepo.UserHasPassword();
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
        ResponseBaseModel changePasswordResult = await UsersProfilesRepo.ChangePassword(Input.OldPassword, Input.NewPassword);
        Messages = changePasswordResult.Messages;
        if (!changePasswordResult.Success())
            return;

        Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = "Ваш пароль был изменен" }];
    }
}