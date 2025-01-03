////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// ChatsTableComponent
/// </summary>
public partial class ChatsTableComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TgRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackBarRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    private IEnumerable<ChatTelegramModelDB> pagedData = [];
    private MudTable<ChatTelegramModelDB> table = default!;

    private string? searchString = null;

    readonly List<UserInfoModel> UsersCache = [];
    readonly Dictionary<string, IssueHelpdeskModel[]> IssuesCache = [];

    async Task<TableData<ChatTelegramModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        await SetBusy();
        TPaginationRequestModel<string?> req = new()
        {
            Payload = searchString,
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        TPaginationResponseModel<ChatTelegramModelDB> rest = await TgRepo.ChatsSelect(req);
        IsBusyProgress = false;

        if (rest.Response is null)
            return new TableData<ChatTelegramModelDB>() { TotalItems = 0, Items = pagedData };
        pagedData = rest.Response;
        await LoadUsersData();
        return new TableData<ChatTelegramModelDB>() { TotalItems = rest.TotalRowsCount, Items = pagedData };
    }

    async Task LoadUsersData()
    {
        IQueryable<ChatTelegramModelDB> q = pagedData
            .Where(x => x.Type == ChatsTypesTelegramEnum.Private && !UsersCache.Any(y => y.TelegramId == x.ChatTelegramId))
            .AsQueryable();

        long[] users_ids_for_load = [.. q.Select(x => x.ChatTelegramId)];
        if (users_ids_for_load.Length == 0)
            return;

        await SetBusy();
        TResponseModel<UserInfoModel[]?> users_res = await WebRepo.GetUserIdentityByTelegram(users_ids_for_load);
        IsBusyProgress = false;
        SnackBarRepo.ShowMessagesResponse(users_res.Messages);
        if (!users_res.Success() || users_res.Response is null || users_res.Response.Length == 0)
            return;

        UsersCache.AddRange(users_res.Response);

        string[] users_ids_identity = [.. users_res.Response.Select(x => x.UserId)];
        await SetBusy();
        TPaginationResponseModel<IssueHelpdeskModel> issues_users_res = await HelpdeskRepo
                    .IssuesSelect(new()
                    {
                        Payload = new()
                        {
                            IdentityUsersIds = [.. users_ids_identity],
                            JournalMode = HelpdeskJournalModesEnum.All,
                            IncludeSubscribers = true,
                        },
                        PageNum = 0,
                        PageSize = int.MaxValue,
                        SortBy = nameof(IssueHelpdeskModel.LastUpdateAt),
                        SortingDirection = VerticalDirectionsEnum.Down,
                    });
        IsBusyProgress = false;

        if (issues_users_res.Response is null || issues_users_res.Response.Count == 0)
            return;

        foreach (UserInfoModel us in users_res.Response)
        {
            IssueHelpdeskModel[] issues_for_user = [.. issues_users_res.Response
                .Where(x =>
                x.ExecutorIdentityUserId == us.UserId ||
                x.Subscribers!.Any(y => y.UserId == us.UserId) ||
                x.AuthorIdentityUserId == us.UserId)];

            if (issues_for_user.Length != 0)
                IssuesCache.Add(us.UserId, issues_for_user);
        }
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
}