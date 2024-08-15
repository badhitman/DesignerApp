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

    async Task NotifyBellToggle(SubscriberIssueHelpdeskModelDB p)
    {
        TAuthRequestModel<SubscribeUpdateRequestModel> req = new()
        {
            Payload = new()
            {
                IssueId = Issue.Id,
                SetValue = true,
                UserId = p.UserId,
                IsSilent = !p.IsSilent,
            },
            SenderActionUserId = CurrentUser.UserId
        };

        IsBusyProgress = true;
        TResponseModel<bool?> rest = await HelpdeskRepo.SubscribeUpdate(req);

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        TResponseModel<SubscriberIssueHelpdeskModelDB[]?> subscribes = await HelpdeskRepo.SubscribesList(new TAuthRequestModel<int>() { Payload = Issue.Id, SenderActionUserId = CurrentUser.UserId });

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(subscribes.Messages);
        Issue.Subscribers = [.. subscribes.Response];
    }

    async Task SubscribeMeToggle()
    {
        TAuthRequestModel<SubscribeUpdateRequestModel> req = new()
        {
            Payload = new()
            {
                IssueId = Issue.Id,
                SetValue = CanSubscribe,
                UserId = CurrentUser.UserId
            },
            SenderActionUserId = CurrentUser.UserId
        };

        IsBusyProgress = true;
        TResponseModel<bool?> rest = await HelpdeskRepo.SubscribeUpdate(req);

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        TResponseModel<SubscriberIssueHelpdeskModelDB[]?> subscribes = await HelpdeskRepo.SubscribesList(new TAuthRequestModel<int>() { Payload = Issue.Id, SenderActionUserId = CurrentUser.UserId });

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(subscribes.Messages);
        Issue.Subscribers = [.. subscribes.Response];
    }
}