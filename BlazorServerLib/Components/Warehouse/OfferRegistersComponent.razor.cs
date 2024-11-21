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
public partial class OfferRegistersComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// CommRepo
    /// </summary>
    [Inject]
    ICommerceRemoteTransmissionService CommRepo { get; set; } = default!;

    private MudTable<OfferAvailabilityModelDB>? table;

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
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>> rest = await CommRepo.OffersRegistersSelect(req);
        await SetBusy(false, token: token);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (rest.Response is null)
            return new TableData<OfferAvailabilityModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<OfferAvailabilityModelDB>() { TotalItems = rest.Response.TotalRowsCount, Items = rest.Response.Response };
    }
}