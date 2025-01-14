////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Users.Pages;

/// <summary>
/// TelegramUsersPage
/// </summary>
public partial class TelegramUsersPage
{
    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;


    [SupplyParameterFromForm]
    private FindRequestModel Input { get; set; } = new() { FindQuery = "" };

    PaginationState pagination = new PaginationState { ItemsPerPage = 15 };
    string? nameFilter;
    QuickGrid<TelegramUserViewModel>? myGrid;
    int numResults;
    GridItemsProvider<TelegramUserViewModel>? foodRecallProvider;

    IEnumerable<ResultMessage>? Messages;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        foodRecallProvider = async req =>
        {
            TPaginationResponseModel<TelegramUserViewModel> res = await IdentityRepo.FindUsersTelegram(new FindRequestModel()
            {
                FindQuery = Input.FindQuery,
                PageNum = pagination.CurrentPageIndex,
                PageSize = pagination.ItemsPerPage
            });

            if (res.Response is null)
            {
                string msg = "TelegramUsers is null. error {204B22F2-A5B3-4EEF-BC9B-494CEBF266E8}";
                Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = msg }];
                throw new Exception(msg);
            }

            if (numResults != res.TotalRowsCount)
            {
                numResults = res.TotalRowsCount;
                StateHasChanged();
            }

            return GridItemsProviderResult.From<TelegramUserViewModel>(res.Response, res.TotalRowsCount);
        };
    }

    async Task UsersFilter()
    {

        if (myGrid is not null)
            await myGrid.RefreshDataAsync();
    }
}