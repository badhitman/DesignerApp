////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// Участники диалога
/// </summary>
public partial class SubscribersIssueComponent : IssueWrapBaseModel
{
    bool CanSubscribe => Issue.Subscribers?.Any(x => x.UserId == CurrentUser.UserId) != true;

    async Task SubscribeMeToggle()
    {
        IsBusyProgress = true;
        TResponseModel<bool?> rest = await HelpdeskRepo.SubscribeUpdate(new()
        {
            Payload = new()
            {
                IssueId = Issue.Id,
                SubscribeSet = CanSubscribe,
                UserId = CurrentUser.UserId
            },
            SenderActionUserId = CurrentUser.UserId
        });

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        TResponseModel<SubscriberIssueHelpdeskModelDB[]?> subscribes = await HelpdeskRepo.SubscribesList(new TAuthRequestModel<int>() { Payload = Issue.Id, SenderActionUserId = CurrentUser.UserId });

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(subscribes.Messages);
        Issue.Subscribers = [.. subscribes.Response];
    }
}