////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// IssueMessagesComponent
/// </summary>
public partial class IssueMessagesComponent : IssueWrapBaseModel
{
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

        Issue.Messages = [..messages_rest.Response];
    }
}