////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharedLib;

namespace BlazorWebLib.Components.Account.Pages;

public partial class LoginPage
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    [SupplyParameterFromForm]
    // #if DEMO
    //     private UserAuthorizationModel Input { get; set; } = new() { Password = "Qwerty123!" };
    //     bool IsDebug = true;
    // #else
    private UserAuthorizationModel Input { get; set; } = new();
    bool IsDebug = false;
    //#endif

    /// <summary>
    /// This doesn't count login failures towards account lockout
    /// To enable password failures to trigger account lockout, set lockoutOnFailure: true
    /// </summary>
    IdentityResultResponseModel? result;

    List<ResultMessage> Messages = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (HttpMethods.IsGet(HttpContext.Request.Method) && HttpContext.User.Identity?.IsAuthenticated == true)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    /// <summary>
    /// LoginUser
    /// </summary>
    public async Task LoginUser()
    {
        result = await UserAuthManage.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe);
        Messages.AddRange(result.Messages);
        if (result.RequiresTwoFactor == true)
        {
            RedirectManager.RedirectTo(
                "Account/LoginWith2fa",
                new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
        }
        else if (result.Success())
        {
            RedirectManager.RedirectTo(ReturnUrl);
        }
        else if (result.IsLockedOut == true)
        {
            RedirectManager.RedirectTo("Account/Lockout");
        }
    }
}