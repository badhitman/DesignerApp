////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Articles;

/// <summary>
/// ArticlesListComponent
/// </summary>
public partial class ArticlesListComponent : BlazorBusyComponentBaseAuthModel
{

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;


    private MudTable<ArticleModelDB> table = default!;

    private string? searchString = null;
    readonly List<UserInfoModel> usersDump = [];

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<ArticleModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        await SetBusy(token: token);
        TPaginationRequestModel<SelectArticlesRequestModel> req = new()
        {
            Payload = new()
            {
                IdentityUsersIds = [CurrentUserSession!.UserId],
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
        await UpdateUsersData(rest.Response.Response!.Select(x => x.AuthorIdentityId).ToArray());
        // Return the data
        return new() { TotalItems = rest.Response.TotalRowsCount, Items = data };
    }

    async Task UpdateUsersData(string?[] users_ids)
    {
        string[] _ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x) && !usersDump.Any(y => y.UserId == x))];
        if (_ids.Length == 0)
            return;

        await SetBusy();

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
}