////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Account.Pages.Manage;

/// <summary>
/// IndexPage
/// </summary>
public partial class IndexPage : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService webRepo { get; set; } = default!;

    UserInfoModel CurrentUser = default!;

    string? username;
    string? firstName;
    string? lastName;

    List<ResultMessage> Messages = [];

    bool IsEdited => firstName != CurrentUser.GivenName || lastName != CurrentUser.Surname;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        IsBusyProgress = false;

        UserInfoMainModel user_state = state.User.ReadCurrentUserInfo() ?? throw new Exception();

        TResponseModel<string?> username_rest = await UsersProfilesRepo.GetUserNameAsync();
        Messages.AddRange(username_rest.Messages);
        username = username_rest.Response;

        IsBusyProgress = true;
        TResponseModel<UserInfoModel[]?> user_data = await webRepo.GetUsersIdentity([user_state.UserId]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(user_data.Messages);
        if (!user_data.Success() || user_data.Response is null)
            throw new Exception();

        CurrentUser = user_data.Response.Single();

        firstName = CurrentUser.GivenName;
        lastName = CurrentUser.Surname;
    }

    private async Task SaveAsync()
    {
        Messages = [];
        IsBusyProgress = true;
        ResponseBaseModel rest = await UsersProfilesRepo.UpdateFirstLastNamesUser(CurrentUser.UserId, firstName, lastName);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success())
        {
            CurrentUser.GivenName = firstName;
            CurrentUser.Surname = lastName;

            // AuthenticationState state = await authRepo.GetAuthenticationStateAsync();

        }
    }
}