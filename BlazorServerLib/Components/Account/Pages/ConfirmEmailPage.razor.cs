using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Account.Pages;

/// <inheritdoc/>
public partial class ConfirmEmailPage : ComponentBase
{
    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;

    [Inject]
    IdentityRedirectManager RedirectManager { get; set; } = default!;


    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    /// <inheritdoc/>
    public List<ResultMessage> Messages = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (UserId is null || Code is null)
        {
            RedirectManager.RedirectTo("");
        }

        TResponseModel<UserInfoModel[]> findUsers = await IdentityRepo.GetUsersIdentity([UserId]);
        Messages = findUsers.Messages;
        if (findUsers.Response is null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        else
        {
            UserInfoModel user = findUsers.Response.Single();
            ResponseBaseModel result = await IdentityRepo.ConfirmUserEmailCode(new() { Code = Code, UserId = user.UserId });
            Messages.AddRange(result.Messages);
            if (!result.Success())
            {
                Messages.Add(new() { TypeMessage = ResultTypesEnum.Success, Text = "Ошибка подтверждения электронной почты." });
            }
        }
    }
}