////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// IssueMessagesComponent
/// </summary>
public partial class IssueMessagesComponent : IssueWrapBaseModel
{
    [Inject]
    IWebRemoteTransmissionService WebRemoteRepo { get; set; } = default!;


    /// <summary>
    /// Добавляется новое сообщение
    /// </summary>
    public bool AddingNewMessage = false;

    /// <summary>
    /// ReloadMessages
    /// </summary>
    public async Task ReloadMessages()
    {
        IsBusyProgress = true;
        TResponseModel<IssueMessageHelpdeskModelDB[]?> messages_rest = await HelpdeskRepo.MessagesList(new()
        {
            Payload = Issue.Id,
            SenderActionUserId = CurrentUser.UserId,
        });
        SnackbarRepo.ShowMessagesResponse(messages_rest.Messages);
        IsBusyProgress = false;
        if (!messages_rest.Success() || messages_rest.Response is null)
            return;

        Issue.Messages = [.. messages_rest.Response];

        string[] users_for_adding = Issue
            .Messages
            .Where(x => !UsersIdentityDump.Any(y => y.UserId == x.AuthorUserId))
            .Select(x => x.AuthorUserId)
            .ToArray();

        if (users_for_adding.Length != 0)
        {
            TResponseModel<UserInfoModel[]?> users_data_identity = await WebRemoteRepo.FindUsersIdentity([.. users_for_adding.Distinct()]);
            SnackbarRepo.ShowMessagesResponse(users_data_identity.Messages);
            if (users_data_identity.Response is not null && users_data_identity.Response.Length != 0)
                UsersIdentityDump.AddRange(users_data_identity.Response);
        }

        StateHasChanged();
    }
}