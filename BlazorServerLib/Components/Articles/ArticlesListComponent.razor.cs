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


    UserInfoMainModel user = default!;
    private IEnumerable<ArticleModelDB> pagedData = [];
    private MudTable<ArticleModelDB> table = default!;

    private int totalItems;
    private string? searchString = null;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<ArticleModelDB>> ServerReload(TableState state, CancellationToken token)
    {//public Task<TResponseModel<List<ArticleModelDB>>> ArticlesSelect(SelectArticlesRequestModel req);

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
        //SnackbarRepo.ShowMessagesResponse(rest.Messages);

        //// Forward the provided token to methods which support it
        //List<ArticleModelDB> data = rest.Response!.Response!;
        //await UpdateUsersData(rest.Response.Response!.SelectMany(x => new string?[] { x.AuthorIdentityUserId, x.ExecutorIdentityUserId }).ToArray());
        //// Return the data
        //return new() { TotalItems = rest.Response.TotalRowsCount, Items = data };

        return new TableData<ArticleModelDB>() { TotalItems = totalItems, Items = pagedData };
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