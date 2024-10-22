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
public partial class IndexPage : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    IWebRemoteTransmissionService webRepo { get; set; } = default!;

    string? username;
    string? firstName;
    string? lastName;

    List<ResultMessage> Messages = [];

    bool IsEdited => CurrentUserSession is not null && (firstName != CurrentUserSession.GivenName || lastName != CurrentUserSession.Surname);

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        TResponseModel<string?> username_rest = await UsersProfilesRepo.GetUserNameAsync();
        Messages.AddRange(username_rest.Messages);
        username = username_rest.Response;

        if (CurrentUserSession is null)
            throw new Exception();

        firstName = CurrentUserSession.GivenName;
        lastName = CurrentUserSession.Surname;
    }

    private async Task SaveAsync()
    {
        if (CurrentUserSession is null)
            throw new ArgumentNullException(nameof(CurrentUserSession));

        Messages = [];
        SetBusy();

        ResponseBaseModel rest = await UsersProfilesRepo.UpdateFirstLastNamesUser(CurrentUserSession.UserId, firstName, lastName);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success())
        {
            CurrentUserSession.GivenName = firstName;
            CurrentUserSession.Surname = lastName;
        }
    }
}