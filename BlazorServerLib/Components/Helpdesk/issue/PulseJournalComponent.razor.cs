////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// PulseJournalComponent
/// </summary>
public partial class PulseJournalComponent : IssueWrapBaseModel
{
    /// <summary>
    /// webRemoteRepo
    /// </summary>
    [Inject]
    IWebRemoteTransmissionService WebRemoteRepo { get; set; } = default!;


    private MudTable<PulseViewModel> table = default!;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<PulseViewModel>> ServerReload(TableState state, CancellationToken token)
    {
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<PulseViewModel>> rest = await HelpdeskRepo.PulseJournal(new TPaginationRequestModel<UserIssueModel>()
        {
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortingDirection = state.SortDirection == SortDirection.Descending ? VerticalDirectionsEnum.Down : VerticalDirectionsEnum.Up,
            SortBy = state.SortLabel,
            Payload = new()
            {
                UserId = CurrentUser.UserId,
                IssueId = Issue.Id,
            }
        });

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        TPaginationResponseModel<PulseViewModel> tp = rest.Response!;

        string[] users_ids = tp.Response!
            .Select(x => x.AuthorUserIdentityId)
            .Where(x => !UsersIdentityDump.Any(y => y.UserId == x))
            .ToArray();

        if (users_ids.Length != 0)
        {
            IsBusyProgress = true;
            TResponseModel<UserInfoModel[]?> users_add = await WebRemoteRepo.GetUsersIdentity(users_ids);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(users_add.Messages);
            if (users_add.Response is not null)
                UsersIdentityDump.AddRange(users_add.Response);
        }

        return new TableData<PulseViewModel>() { TotalItems = tp.TotalRowsCount, Items = tp.Response };
    }
}