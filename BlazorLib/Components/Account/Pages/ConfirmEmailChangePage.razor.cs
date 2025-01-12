using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using SharedLib;
using System.Text;

namespace BlazorLib.Components.Account.Pages;

/// <summary>
/// ConfirmEmailChange
/// </summary>
public partial class ConfirmEmailChangePage : ComponentBase
{

    [Inject]
    IdentityRedirectManager RedirectManager { get; set; } = default!;

    [Inject]
    IUsersProfilesService UserProfilesManage { get; set; } = default!;

    [Inject]
    IUsersAuthenticateService UserAuthManager { get; set; } = default!;


    [SupplyParameterFromQuery]
    private string? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Email { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    List<ResultMessage> Messages = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (UserId is null || Email is null || Code is null)
        {
            Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = "Ошибка: неверная ссылка для подтверждения изменения адреса электронной почты." }];
            return;
        }

        TResponseModel<UserInfoModel?> user = await UserProfilesManage.FindByIdAsync(UserId);
        Messages = user.Messages;
        if (user.Response is null)
        {
            Messages.Add(new() { TypeMessage = ResultTypesEnum.Error, Text = $"Невозможно найти пользователя по идентификатору '{UserId}'" });
            return;
        }

        string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        ResponseBaseModel result = await UserProfilesManage.ChangeEmailAsync(new() { Email = Email, Token = code, UserId = user.Response.UserId });
        Messages = result.Messages;
    }
}