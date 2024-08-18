////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// PulseJournalComponent
/// </summary>
public partial class PulseJournalComponent : IssueWrapBaseModel
{
    private MudTable<PulseViewModel> table = default!;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<PulseViewModel>> ServerReload(TableState state, CancellationToken token)
    {
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<PulseViewModel>?> rest = await this.HelpdeskRepo.PulseJournal(new TPaginationRequestModel<UserIssueModel>()
        {
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortingDirection = state.SortDirection == SortDirection.Descending ? VerticalDirectionsEnum.Down : VerticalDirectionsEnum.Up,
            Payload = new()
            {
                UserId = CurrentUser.UserId,
                IssueId = Issue.Id,
            }
        });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        TPaginationResponseModel<PulseViewModel> tp = rest.Response!;

        return new TableData<PulseViewModel>() { TotalItems = tp.TotalRowsCount, Items = tp.Response };
    }
}