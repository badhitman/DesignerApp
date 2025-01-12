////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.Account.Pages;

/// <summary>
/// ForgotPasswordPage
/// </summary>
public partial class ForgotPasswordPage
{
    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    IdentityRedirectManager RedirectManager { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;

    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;


    [SupplyParameterFromForm]
    private EmailSingleModel Input { get; set; } = new();

    List<ResultMessage> Messages = [];

    private async Task OnValidSubmitAsync()
    {
        TResponseModel<UserInfoModel>? user = await IdentityRepo.FindUserByEmail(Input.Email);
        if (user.Response is null)
            RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");

        UserBooleanResponseModel email_is_confirmed_rest = await UsersProfilesRepo.IsEmailConfirmedAsync(user.Response.UserId);
        if (user is null || !email_is_confirmed_rest.Success() || email_is_confirmed_rest.Response != true)
        {
            // Don't reveal that the user does not exist or is not confirmed
            RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
        }
        Messages.AddRange(email_is_confirmed_rest.Messages);

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        TResponseModel<string?> code_rest = await UsersProfilesRepo.GeneratePasswordResetTokenAsync(user.Response.UserId);
        Messages.AddRange(code_rest.Messages);
        if (string.IsNullOrEmpty(code_rest.Response))
            throw new Exception("PasswordResetToken is null. error {92318C2E-0997-4737-B746-73698FB38B39}");

        ResponseBaseModel send_pass_reset_rest = await UsersProfilesRepo.SendPasswordResetLinkAsync(Input.Email, NavigationManager.ToAbsoluteUri("Account/ResetPassword").AbsoluteUri, code_rest.Response, user.Response.UserId);
        Messages.AddRange(send_pass_reset_rest.Messages);
        RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
    }
}