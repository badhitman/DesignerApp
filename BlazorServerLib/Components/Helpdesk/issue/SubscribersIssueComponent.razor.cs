////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using System.Net.Mail;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// Участники диалога
/// </summary>
public partial class SubscribersIssueComponent : IssueWrapBaseModel
{
    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;


    bool CanSubscribe => Issue.Subscribers?.Any(x => x.UserId == CurrentUserSession!.UserId) != true;

    string? addingSubscriber;

    async Task AddSubscriber()
    {
        if (!MailAddress.TryCreate(addingSubscriber, out _))
            throw new Exception();

        await SetBusy();

        UserInfoModel? user_by_email = await UsersProfilesRepo.FindByEmailAsync(addingSubscriber);
        IsBusyProgress = false;
        if (user_by_email is null)
        {
            SnackbarRepo.Error($"Пользователь с таким email не найден: {addingSubscriber}");
            return;
        }

        if (!UsersIdentityDump.Any(x => x.UserId == user_by_email.UserId))
            UsersIdentityDump.Add(user_by_email);

        await SetBusy();

        TResponseModel<bool> add_subscriber_res = await HelpdeskRepo.SubscribeUpdate(new()
        {
            SenderActionUserId = CurrentUserSession!.UserId,
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
        await SetBusy();
        TResponseModel<List<SubscriberIssueHelpdeskModelDB>> res = await HelpdeskRepo.SubscribesList(new() { Payload = Issue.Id, SenderActionUserId = CurrentUserSession!.UserId });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        Issue.Subscribers = [.. res.Response];
        IsBusyProgress = false;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
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
            SenderActionUserId = CurrentUserSession!.UserId
        };

        await SetBusy();

        TResponseModel<bool> rest = await HelpdeskRepo.SubscribeUpdate(req);

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            IsBusyProgress = false;
            return;
        }

        TResponseModel<List<SubscriberIssueHelpdeskModelDB>> res = await HelpdeskRepo.SubscribesList(new TAuthRequestModel<int>() { Payload = Issue.Id, SenderActionUserId = CurrentUserSession!.UserId });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        Issue.Subscribers = [.. res.Response];
        IsBusyProgress = false;
    }

    async Task SubscribeMeToggle()
    {
        TAuthRequestModel<SubscribeUpdateRequestModel> req = new()
        {
            Payload = new()
            {
                IssueId = Issue.Id,
                SetValue = CanSubscribe,
                UserId = CurrentUserSession!.UserId
            },
            SenderActionUserId = CurrentUserSession!.UserId
        };

        await SetBusy();

        TResponseModel<bool> rest = await HelpdeskRepo.SubscribeUpdate(req);

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;
        TResponseModel<List<SubscriberIssueHelpdeskModelDB>> res = await HelpdeskRepo.SubscribesList(new TAuthRequestModel<int>() { Payload = Issue.Id, SenderActionUserId = CurrentUserSession!.UserId });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        Issue.Subscribers = [.. res.Response];
        IsBusyProgress = false;
    }
}