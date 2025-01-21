////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Account.Pages.Manage;

/// <summary>
/// SetPasswordPage
/// </summary>
public partial class SetPasswordPage
{
    [Inject]
    IdentityRedirectManager RedirectManager { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;


    [SupplyParameterFromForm]
    private SetNewPasswordModel Input { get; set; } = new();

    List<ResultMessage>? Messages;
    private UserInfoModel user = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        UserBooleanResponseModel hasPassword = await UsersProfilesRepo.UserHasPassword();
        Messages = hasPassword.Messages;
        if (hasPassword.Response == true || hasPassword.UserInfo is null)
        {
            RedirectManager.RedirectTo("Account/Manage/ChangePassword");
        }
        user = hasPassword.UserInfo;
    }

    private async Task OnValidSubmitAsync()
    {
        Messages = null;
        ResponseBaseModel rest = await UsersProfilesRepo.AddPassword(Input.NewPassword!);
        Messages = rest.Messages;
        if (!rest.Success())
            return;
        Messages.Add(new() { TypeMessage = ResultTypesEnum.Warning, Text = "Ваш пароль установлен." });
    }
}