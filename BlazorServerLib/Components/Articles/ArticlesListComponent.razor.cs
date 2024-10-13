////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebLib.Components.Articles;

/// <summary>
/// ArticlesListComponent
/// </summary>
public partial class ArticlesListComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;


    UserInfoMainModel user = default!;
    private IEnumerable<ArticleModelDB> pagedData = [];
    private MudTable<ArticleModelDB> table = default!;

    private int totalItems;
    private string? searchString = null;
    readonly List<UserInfoModel> usersDump = [];

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<ArticleModelDB>> ServerReload(TableState state, CancellationToken token)
    {

        IsBusyProgress = true;
        await Task.Delay(1, token);
        TPaginationRequestModel<SelectArticlesRequestModel> req = new()
        {
            Payload = new()
            {
                IdentityUsersIds = [user.UserId],
                SearchQuery = searchString,
                IncludeExternal = true,
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };

        TResponseModel<TPaginationResponseModel<ArticleModelDB>> rest = await HelpdeskRepo
            .ArticlesSelect(req);

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        // Forward the provided token to methods which support it
        List<ArticleModelDB> data = rest.Response!.Response!;
        await UpdateUsersData(rest.Response.Response!.SelectMany(x => new string?[] { x.AuthorIdentityId }).ToArray());
        // Return the data
        return new() { TotalItems = rest.Response.TotalRowsCount, Items = data };
    }

    async Task UpdateUsersData(string?[] users_ids)
    {
        string[] _ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x) && !usersDump.Any(y => y.UserId == x))];
        if (_ids.Length == 0)
            return;

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<UserInfoModel[]?> res = await WebRepo.GetUsersIdentity(_ids);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Response is null)
            return;
        usersDump.AddRange(res.Response);
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
    }
}