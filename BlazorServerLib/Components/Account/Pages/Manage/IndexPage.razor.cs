////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebLib.Components.Account.Pages.Manage;

/// <summary>
/// IndexPage
/// </summary>
public partial class IndexPage : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    IWebRemoteTransmissionService webRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;

    string? username;
    string? firstName;
    string? lastName;
    string? phoneNum;

    List<ResultMessage> Messages = [];

    bool IsEdited => CurrentUserSession is not null && (firstName != CurrentUserSession.GivenName || lastName != CurrentUserSession.Surname || phoneNum != CurrentUserSession.PhoneNumber);

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReadCurrentUser();
        TResponseModel<string?> username_rest = await UsersProfilesRepo.GetUserNameAsync();
        Messages.AddRange(username_rest.Messages);
        username = username_rest.Response;

        if (CurrentUserSession is null)
            throw new Exception();

        firstName = CurrentUserSession.GivenName;
        lastName = CurrentUserSession.Surname;
        phoneNum = CurrentUserSession.PhoneNumber;
    }

    private async Task SaveAsync()
    {
        if (CurrentUserSession is null)
            throw new ArgumentNullException(nameof(CurrentUserSession));

        if(!string.IsNullOrWhiteSpace(phoneNum) && !GlobalTools.IsPhoneNumber(phoneNum))
        {
            SnackbarRepo.Error("Телефон должен быть в формате: +79994440011");
            return;
        }

        Messages = [];
        await SetBusy();

        ResponseBaseModel rest = await UsersProfilesRepo.UpdateFirstLastNamesUser(CurrentUserSession.UserId, firstName, lastName, phoneNum);
        AuthenticationState ar = await AuthRepo.GetAuthenticationStateAsync();
        // ar.User.Claims.ToList().ForEach(x => { x. });
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success())
        {
            CurrentUserSession.GivenName = firstName;
            CurrentUserSession.Surname = lastName;
            CurrentUserSession.PhoneNumber = phoneNum;
        }
    }
}