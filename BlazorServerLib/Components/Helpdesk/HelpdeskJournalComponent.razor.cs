////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// 
/// </summary>
public partial class HelpdeskJournalComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    ///Journal mode
    /// </summary>
    [Parameter, EditorRequired]
    public required HelpdeskJournalModesEnum JournalMode { get; set; }

    /// <summary>
    /// SetTab
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required Action<HelpdeskJournalComponent> SetTab { get; set; }

    private string? searchString = null;

    /// <inheritdoc/>
    public MudTable<IssueHelpdeskModel> TableRef = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        SetTab(this);
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server, with a token for canceling this request
    /// </summary>
    private async Task<TableData<IssueHelpdeskModel>> ServerReload(TableState state, CancellationToken token)
    {
        IsBusyProgress = true;

        TResponseModel<UserInfoModel?> _current_user = await UsersProfilesRepo.FindByIdAsync();
        if (!_current_user.Success() || _current_user.Response is null)
        {
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(_current_user.Messages);
            throw new Exception();
        }

        TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?> rest = await HelpdeskRepo
            .IssuesSelect(new GetIssuesForUserRequestModel() { JournalMode = JournalMode, Request = new UserCrossIdsModel() { IdentityUserId = _current_user.Response.UserId }, SearchQuery = searchString });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        // Forward the provided token to methods which support it
        List<IssueHelpdeskModel> data = rest.Response!.Response!;
        // Return the data
        return new () { TotalItems = rest.Response.TotalRowsCount, Items = data };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        TableRef.ReloadServerData();
    }
}