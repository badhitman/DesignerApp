﻿////////////////////////////////////////////////
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
    IHelpdeskTransmission HelpdeskRepo { get; set; } = default!;

    [Inject]
    ICommerceTransmission CommRepo { get; set; } = default!;

    [Inject]
    IStorageTransmission StorageTransmissionRepo { get; set; } = default!;

    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;


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
    RecordsAttendanceModelDB[]? OrdersAttendancesJournal;
    bool ShowingTelegramArea;
    bool ShowingWappiArea;
    bool ShowingAttachmentsIssueArea;

    void ReloadRecords(RecordsAttendanceModelDB[] sender)
    {
        OrdersAttendancesJournal = sender;
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        await Task.WhenAll([
                Task.Run(async () => { TResponseModel<bool?> res = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ShowingTelegramArea); ShowingTelegramArea = res.Response == true; }),
                Task.Run(async () => { TResponseModel<bool?> res = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ShowingAttachmentsIssuesArea); ShowingAttachmentsIssueArea = res.Response == true; }),
                Task.Run(async () => { TResponseModel<bool?> res = await StorageTransmissionRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ShowingWappiArea); ShowingWappiArea = res.Response == true; }),
                ReadCurrentUser(),
                FindOrders(),
                FindOrdersAttendances(),
            ]);
        await ReadIssue();
        await FlushUsersDump();
        await SetBusy(false);
    }

    async Task FindOrders()
    {
        OrdersByIssuesSelectRequestModel req = new()
        {
            IncludeExternalData = true,
            IssueIds = [Id],
        };

        TResponseModel<OrderDocumentModelDB[]> res = await CommRepo.OrdersByIssues(req);

        SnackbarRepo.ShowMessagesResponse(res.Messages);
        OrdersJournal = res.Response is null
            ? null
            : [.. res.Response];
    }

    async Task FindOrdersAttendances()
    {
        OrdersByIssuesSelectRequestModel req = new()
        {
            IncludeExternalData = true,
            IssueIds = [Id],
        };

        TResponseModel<RecordsAttendanceModelDB[]> res = await CommRepo.OrdersAttendancesByIssues(req);

        SnackbarRepo.ShowMessagesResponse(res.Messages);
        OrdersAttendancesJournal = res.Response is null
            ? null
            : [.. res.Response];
    }

    async Task ReadIssue()
    {
        TAuthRequestModel<IssuesReadRequestModel> req = new()
        {
            Payload = new() { IssuesIds = [Id] },
            SenderActionUserId = CurrentUserSession!.UserId
        };

        await SetBusy();
        TResponseModel<IssueHelpdeskModelDB[]> issue_res = await HelpdeskRepo.IssuesRead(req);
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

        TResponseModel<UserInfoModel[]> users_data_identity = await IdentityRepo.GetUsersIdentity([.. users_ids]);
        SnackbarRepo.ShowMessagesResponse(users_data_identity.Messages);
        if (users_data_identity.Response is not null && users_data_identity.Response.Length != 0)
            UsersIdentityDump.AddRange(users_data_identity.Response);
    }
}