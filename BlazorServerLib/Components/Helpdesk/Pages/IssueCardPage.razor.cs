////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.Pages;

/// <summary>
/// IssueCardPage
/// </summary>
public partial class IssueCardPage : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRemoteRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo { get; set; } = default!;


    /// <summary>
    /// Id
    /// </summary>
    [Parameter, EditorRequired]
    public int Id { get; set; }

    bool CanEdit =>
        CurrentUserSession!.IsAdmin ||
        CurrentUserSession!.Roles?.Contains(GlobalStaticConstants.Roles.HelpDeskTelegramBotManager) == true ||
        CurrentUserSession!.UserId == IssueSource?.ExecutorIdentityUserId ||
        CurrentUserSession!.UserId == IssueSource?.AuthorIdentityUserId;

    IssueHelpdeskModelDB? IssueSource { get; set; }

    /// <summary>
    /// UsersIdentityDump
    /// </summary>
    public List<UserInfoModel> UsersIdentityDump = [];

    OrderDocumentModelDB[]? OrdersJournal;
    bool ShowingTelegramArea;
    

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReadCurrentUser();
        await ReadIssue();
        await FlushUsersDump();
        await FindOrders();
        await SetBusy();
        TResponseModel<bool?> res = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ShowingTelegramArea);
        ShowingTelegramArea = res.Response == true;        
        await SetBusy(false);
    }

    async Task FindOrders()
    {
        if (IssueSource is null)
            return;

        OrdersByIssuesSelectRequestModel req = new()
        {
            IncludeExternalData = true,
            IssueIds = [IssueSource.Id],
        };
        await SetBusy();

        TResponseModel<OrderDocumentModelDB[]> res = await CommRepo.OrdersByIssues(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        OrdersJournal = res.Response is null
            ? null
            : [.. res.Response];
    }

    async Task ReadIssue()
    {
        await SetBusy();
        TResponseModel<IssueHelpdeskModelDB[]> issue_res = await HelpdeskRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>() { Payload = new() { IssuesIds = [Id] }, SenderActionUserId = CurrentUserSession!.UserId });
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

        await SetBusy();

        TResponseModel<UserInfoModel[]?> users_data_identity = await WebRemoteRepo.GetUsersIdentity([.. users_ids]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(users_data_identity.Messages);
        if (users_data_identity.Response is not null && users_data_identity.Response.Length != 0)
            UsersIdentityDump.AddRange(users_data_identity.Response);
    }
}