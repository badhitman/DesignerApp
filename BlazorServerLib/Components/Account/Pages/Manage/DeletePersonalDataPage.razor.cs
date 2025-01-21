////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Account.Pages.Manage;

/// <summary>
/// DeletePersonalDataPage
/// </summary>
public partial class DeletePersonalDataPage : BlazorBusyComponentBaseAuthModel
{
    [SupplyParameterFromForm]
    private PasswordSingleModel Input { get; set; } = new();


    bool requirePassword;

    List<ResultMessage> Messages = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Input ??= new();
        var rest = await UsersProfilesRepo.UserHasPassword();
        Messages = rest.Messages;
        requirePassword = rest.Response == true;
    }

    private async Task OnValidSubmitAsync()
    {
        var rest = await UsersProfilesRepo.CheckUserPassword(Input.Password);
        Messages = rest.Messages;
        if (requirePassword && !rest.Success())
            return;

        ResponseBaseModel result = await UsersProfilesRepo.DeleteUserData(Input.Password);
        Messages.AddRange(result.Messages);
        if (!result.Success())
        {
            throw new InvalidOperationException("Произошла непредвиденная ошибка при удалении пользователя.");
        }

        await UserAuthRepo.SignOutAsync();
        RedirectManager.RedirectToCurrentPage();
    }
}