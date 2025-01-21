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

    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;


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

        TResponseModel<UserInfoModel[]> findUsers = await IdentityRepo.GetUsersIdentity([UserId]);
        Messages = findUsers.Messages;
        if (findUsers.Response is null)
        {
            Messages.Add(new() { TypeMessage = ResultTypesEnum.Error, Text = $"Невозможно найти пользователя по идентификатору '{UserId}'" });
            return;
        }

        UserInfoModel user = findUsers.Response.Single();

        string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        ResponseBaseModel result = await UserProfilesManage.ChangeEmail(new() { Email = Email, Token = code, UserId = user.UserId });
        Messages = result.Messages;
    }
}