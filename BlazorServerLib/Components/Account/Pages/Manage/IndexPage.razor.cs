////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Account.Pages.Manage;
//BlazorWebLib.Components.Account.Pages.Manage.IndexPage
public partial class IndexPage : ComponentBase
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;


    UserInfoMainModel? user;

    string? username;
    string? firstName;
    string? lastName;

    List<ResultMessage> Messages = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo();
        if (user is null)
            throw new Exception();

        TResponseModel<string?> username_rest = await UsersProfilesRepo.GetUserNameAsync();
        Messages.AddRange(username_rest.Messages);
        username = username_rest.Response;

        firstName = user.GivenName;
        lastName = user.Surname;

        TResponseModel<string?> phone_number_rest = await UsersProfilesRepo.GetPhoneNumberAsync();
        Messages.AddRange(phone_number_rest.Messages);
    }

    private async Task OnValidSubmitAsync()
    {
        if (user is null)
            throw new Exception();

        Messages = [];
        ResponseBaseModel rest;
        rest = await UsersProfilesRepo.UpdateFirstLastNamesUser(user.UserId, firstName, lastName);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
    }
}