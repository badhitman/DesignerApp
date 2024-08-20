﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// HelpdeskJournalComponent
/// </summary>
public partial class HelpdeskJournalComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    ///Journal mode
    /// </summary>
    [Parameter, EditorRequired]
    public required HelpdeskJournalModesEnum JournalMode { get; set; }

    /// <summary>
    /// UserArea
    /// </summary>
    [Parameter, EditorRequired]
    public required UsersAreasHelpdeskEnum UserArea { get; set; }

    /// <summary>
    /// SetTab
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required Action<HelpdeskJournalComponent> SetTab { get; set; }

    /// <summary>
    /// SetArea
    /// </summary>
    public void SetArea(UsersAreasHelpdeskEnum set)
    {
        UserArea = set;
    }

    private string? searchString = null;

    UserInfoModel CurrentUser = default!;
    List<UserInfoModel> usersDump = [];

    /// <inheritdoc/>
    public MudTable<IssueHelpdeskModel> TableRef = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        SetTab(this);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;

        TResponseModel<UserInfoModel?> _current_user = await UsersProfilesRepo.FindByIdAsync();
        if (!_current_user.Success() || _current_user.Response is null)
        {
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(_current_user.Messages);
            throw new Exception();
        }

        IsBusyProgress = false;
        CurrentUser = _current_user.Response;
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server, with a token for canceling this request
    /// </summary>
    private async Task<TableData<IssueHelpdeskModel>> ServerReload(TableState state, CancellationToken token)
    {
        IsBusyProgress = true;
        TPaginationRequestModel<GetIssuesForUserRequestModel> req = new()
        {
            Payload = new()
            {
                IdentityUserId = CurrentUser.UserId,
                JournalMode = JournalMode,
                SearchQuery = searchString,
                UserArea = UserArea
            }
        };

        TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?> rest = await HelpdeskRepo
            .IssuesSelect(req);

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        // Forward the provided token to methods which support it
        List<IssueHelpdeskModel> data = rest.Response!.Response!;
        await UpdateUsersData(rest.Response.Response!.SelectMany(x => new string?[] { x.AuthorIdentityUserId, x.ExecutorIdentityUserId }).ToArray());
        // Return the data
        return new() { TotalItems = rest.Response.TotalRowsCount, Items = data };
    }

    async Task UpdateUsersData(string?[] users_ids)
    {
        string[] _ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x) && !usersDump.Any(y => y.UserId == x))];
        if (_ids.Length == 0)
            return;

        IsBusyProgress = true;
        TResponseModel<UserInfoModel[]?> res = await WebRepo.FindUsersIdentity(_ids);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Response is null)
            return;
        usersDump.AddRange(res.Response);
        //
    }

    private void OnSearch(string text)
    {
        searchString = text;
        TableRef.ReloadServerData();
    }
}