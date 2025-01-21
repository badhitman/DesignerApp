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
public partial class SetPasswordPage : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    IdentityRedirectManager RedirectManager { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;


    [SupplyParameterFromForm]
    private SetNewPasswordModel Input { get; set; } = new();

    List<ResultMessage>? Messages;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        TResponseModel<bool?> hasPassword = await UsersProfilesRepo.UserHasPassword();
        Messages = hasPassword.Messages;
        if (hasPassword.Response == true || CurrentUserSession is null)
        {
            RedirectManager.RedirectTo("Account/Manage/ChangePassword");
        }
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