////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.Pages;

/// <summary>
/// IssueCardPage
/// </summary>
public partial class IssueCardPage : BlazorBusyComponentBaseModel
{
    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRemoteRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Id
    /// </summary>
    [Parameter, EditorRequired]
    public int Id { get; set; }

    bool CanEdit =>
        CurrentUser.IsAdmin ||
        CurrentUser.Roles?.Contains(GlobalStaticConstants.Roles.HelpDeskTelegramBotManager) == true ||
        CurrentUser.UserId == IssueSource?.ExecutorIdentityUserId ||
        CurrentUser.UserId == IssueSource?.AuthorIdentityUserId;

    UserInfoMainModel CurrentUser { get; set; } = default!;
    IssueHelpdeskModelDB? IssueSource { get; set; }

    /// <summary>
    /// UsersIdentityDump
    /// </summary>
    public List<UserInfoModel> UsersIdentityDump = [];

    OrderDocumentModelDB[]? OrdersJournal;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReadSessionUser();
        await ReadIssue();
        await FlushUsersDump();
        await FindOrders();
    }

    async Task FindOrders()
    {
        if (IssueSource is null)
            return;

        TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req = new()
        {
            PageNum = 0,
            PageSize = int.MaxValue,
            Payload = new()
            {
                Payload = new()
                {
                    IncludeExternalData = true,
                    IssueId = IssueSource.Id,
                },
                SenderActionUserId = CurrentUser.UserId,
            }
        };
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>> res = await CommRepo.OrdersSelect(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        OrdersJournal = res.Response is null
            ? null
            : [.. res.Response.Response];
    }

    async Task ReadSessionUser()
    {
        IsBusyProgress = true;
        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        CurrentUser = state.User.ReadCurrentUserInfo() ?? throw new Exception();
        IsBusyProgress = false;
    }

    async Task ReadIssue()
    {
        IsBusyProgress = true;
        TResponseModel<IssueHelpdeskModelDB[]> issue_res = await HelpdeskRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>() { Payload = new() { IssuesIds = [Id] }, SenderActionUserId = CurrentUser.UserId });
        SnackbarRepo.ShowMessagesResponse(issue_res.Messages);
        IssueSource = issue_res.Response?.FirstOrDefault();
        IsBusyProgress = false;
    }

    async Task FlushUsersDump()
    {
        if (IssueSource is null)
            return;

        List<string> users_ids = [IssueSource.AuthorIdentityUserId!];
        if (!string.IsNullOrWhiteSpace(IssueSource.ExecutorIdentityUserId))
            users_ids.Add(IssueSource.ExecutorIdentityUserId);

        if (IssueSource.Subscribers is not null && IssueSource.Subscribers.Count != 0)
            users_ids.AddRange(IssueSource.Subscribers.Select(x => x.UserId));

        if (IssueSource.Messages is not null && IssueSource.Messages.Count != 0)
            users_ids.AddRange(IssueSource.Messages.Select(x => x.AuthorUserId));

        users_ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x))];
        if (users_ids.Count != 0)

            users_ids = users_ids.Distinct().ToList();
        IsBusyProgress = true;
        TResponseModel<UserInfoModel[]?> users_data_identity = await WebRemoteRepo.GetUsersIdentity([.. users_ids]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(users_data_identity.Messages);
        if (users_data_identity.Response is not null && users_data_identity.Response.Length != 0)
            UsersIdentityDump.AddRange(users_data_identity.Response);
    }
}