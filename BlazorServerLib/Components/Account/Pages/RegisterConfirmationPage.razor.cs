////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Account.Pages;

/// <summary>
/// RegisterConfirmationPage
/// </summary>
public partial class RegisterConfirmationPage
{
    [Inject]
    IdentityRedirectManager RedirectManager { get; set; } = default!;

    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;


    [SupplyParameterFromQuery]
    private string? Email { get; set; }

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    IEnumerable<ResultMessage>? Messages;

/// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (Email is null)
        {
            RedirectManager.RedirectTo("");
        }

        TResponseModel<UserInfoModel> user = await IdentityRepo.FindUserByEmail(Email);
        if (user.Response is null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            string msg = "Ошибка поиска пользователя по адресу электронной почты.";
            Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = msg }];
        }
    }
}