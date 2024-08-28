////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;
using System.Net.Mail;
using static MudBlazor.Colors;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// Участники диалога
/// </summary>
public partial class SubscribersIssueComponent : IssueWrapBaseModel
{
    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;


    bool CanSubscribe => Issue.Subscribers?.Any(x => x.UserId == CurrentUser.UserId) != true;

    string? addingSubscriber;

    async Task AddSubscriber()
    {
        if (!MailAddress.TryCreate(addingSubscriber, out _))
            throw new Exception();

        IsBusyProgress = true;
        UserInfoModel? user_by_email = await UsersProfilesRepo.FindByEmailAsync(addingSubscriber);
        IsBusyProgress = false;
        if (user_by_email is null)
        {
            SnackbarRepo.Error($"Пользователь с таким email не найден: {addingSubscriber}");
            return;
        }

        if (!UsersIdentityDump.Any(x => x.UserId == user_by_email.UserId))
            UsersIdentityDump.Add(user_by_email);

        IsBusyProgress = true;
        TResponseModel<bool> add_subscriber_res = await HelpdeskRepo.SubscribeUpdate(new()
        {
            SenderActionUserId = CurrentUser.UserId,
            Payload = new()
            {
                SetValue = true,
                IssueId = Issue.Id,
                UserId = user_by_email.UserId,
            }
        });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(add_subscriber_res.Messages);
        if (!add_subscriber_res.Success() || add_subscriber_res.Response != true)
            return;

        addingSubscriber = null;
        IsBusyProgress = true;
        TResponseModel<SubscriberIssueHelpdeskModelDB[]?> reload_subscribers_list = await HelpdeskRepo.SubscribesList(new() { Payload = Issue.Id, SenderActionUserId = CurrentUser.UserId });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(reload_subscribers_list.Messages);
        if (!reload_subscribers_list.Success() || reload_subscribers_list.Response is null)
            return;

        Issue.Subscribers = [.. reload_subscribers_list.Response];
    }



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
        TResponseModel<bool> rest = await HelpdeskRepo.SubscribeUpdate(req);

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
        TResponseModel<bool> rest = await HelpdeskRepo.SubscribeUpdate(req);

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        TResponseModel<SubscriberIssueHelpdeskModelDB[]?> subscribes = await HelpdeskRepo.SubscribesList(new TAuthRequestModel<int>() { Payload = Issue.Id, SenderActionUserId = CurrentUser.UserId });

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(subscribes.Messages);
        Issue.Subscribers = [.. subscribes.Response];
    }
}