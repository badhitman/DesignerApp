using BlazorLib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebLib.Components.Account.Pages;

/// <summary>
/// 
/// </summary>
public partial class ConfirmEmail : ComponentBase
{
    [Inject]
    IUsersProfilesService UserProfilesManage { get; set; } = default!;

    [Inject]
    IUsersAuthenticateService UserAuthManager { get; set; } = default!;

    [Inject]
    IdentityRedirectManager RedirectManager { get; set; } = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<ResultMessage> Messages = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (UserId is null || Code is null)
        {
            RedirectManager.RedirectTo("");
        }

        TResponseModel<UserInfoModel?> user = await UserProfilesManage.FindByIdAsync(UserId);
        Messages = user.Messages;
        if (user.Response is null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        else
        {
            ResponseBaseModel result = await UserAuthManager.ConfirmEmailAsync(user.Response.UserId, Code);
            Messages.AddRange(result.Messages);
            if (!result.Success())
            {
                Messages.Add(new() { TypeMessage = ResultTypesEnum.Success, Text = "Ошибка подтверждения электронной почты." });
            }
        }
    }
}
