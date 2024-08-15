////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using System.Collections.Generic;

namespace BlazorWebLib.Components.Helpdesk.Pages;

/// <summary>
/// IssueCardPage
/// </summary>
public partial class IssueCardPage : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRemoteRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Id
    /// </summary>
    [Parameter, EditorRequired]
    public int Id { get; set; }


    UserInfoModel CurrentUser { get; set; } = default!;
    IssueHelpdeskModelDB? IssueSource { get; set; }

    UserInfoModel[]? usersIdentityDump = null;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<UserInfoModel?> user = await UsersProfilesRepo.FindByIdAsync();
        SnackbarRepo.ShowMessagesResponse(user.Messages);
        CurrentUser = user.Response ?? throw new Exception();
        TResponseModel<IssueHelpdeskModelDB?> issue_res = await HelpdeskRepo.IssueRead(new TAuthRequestModel<int>() { Payload = Id, SenderActionUserId = CurrentUser.UserId });
        SnackbarRepo.ShowMessagesResponse(issue_res.Messages);
        IssueSource = issue_res.Response;

        if (issue_res.Success() && IssueSource is not null)
        {
            List<string> users_ids = [IssueSource.AuthorIdentityUserId!];
            if (!string.IsNullOrWhiteSpace(IssueSource.ExecutorIdentityUserId))
                users_ids.Add(IssueSource.ExecutorIdentityUserId);

            if (IssueSource.Subscribers is not null && IssueSource.Subscribers.Count != 0)
                users_ids.AddRange(IssueSource.Subscribers.Select(x => x.UserId));

            if (users_ids.Count != 0)
            {
                users_ids = users_ids.Distinct().ToList();
                TResponseModel<UserInfoModel[]?> users_data_identity = await WebRemoteRepo.FindUsersIdentity([.. users_ids]);
                SnackbarRepo.ShowMessagesResponse(users_data_identity.Messages);
                usersIdentityDump = users_data_identity.Response;
            }
        }

        IsBusyProgress = false;
    }
}