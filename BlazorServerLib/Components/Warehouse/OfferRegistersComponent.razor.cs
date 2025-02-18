////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Warehouse;

/// <summary>
/// OfferRegistersComponent
/// </summary>
public partial class OfferRegistersComponent : BlazorBusyComponentRubricsCachedModel
{
    private MudTable<OfferAvailabilityModelDB>? table;

    async Task ReloadTable()
    {
        if (table is null)
            return;

        await SetBusy();
        await table.ReloadServerData();
        await SetBusy(false);
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OfferAvailabilityModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        await SetBusy(token: token);
        TPaginationRequestModel<RegistersSelectRequestBaseModel> req = new()
        {
            Payload = new()
            {

            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? DirectionsEnum.Up : DirectionsEnum.Down,
        };
        TPaginationResponseModel<OfferAvailabilityModelDB> rest = await CommerceRepo.OffersRegistersSelect(req);

        if (rest.Response is not null)
        {
            await CacheRubricsUpdate(rest.Response.Select(x => x.WarehouseId));
            await SetBusy(false, token: token);
            return new TableData<OfferAvailabilityModelDB>() { TotalItems = rest.TotalRowsCount, Items = rest.Response };
        }

        await SetBusy(false, token: token);
        return new TableData<OfferAvailabilityModelDB>() { TotalItems = 0, Items = [] };
    }
}