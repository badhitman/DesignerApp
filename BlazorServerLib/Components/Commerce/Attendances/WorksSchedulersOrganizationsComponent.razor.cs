////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorksSchedulersOrganizationsComponent
/// </summary>
public partial class WorksSchedulersOrganizationsComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// Offer
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required OfferModelDB? Offer { get; set; }

    OfferModelDB? OfferCurrent;
    private MudTable<OrganizationModelDB>? table;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OrganizationModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        if (CurrentUserSession is null)
            return new TableData<OrganizationModelDB>() { TotalItems = 0, Items = [] };

        TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req = new()
        {
            Payload = new()
            {
                // ForUserIdentityId = _filterUser,
                
            },
            SenderActionUserId = CurrentUserSession.UserId,
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        await SetBusy(token: token);
        TResponseModel<TPaginationResponseModel<OrganizationModelDB>> res = await CommerceRepo.OrganizationsSelect(req);
        await SetBusy(false, token: token);
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (!res.Success() || res.Response?.Response is null)
            return new TableData<OrganizationModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<OrganizationModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }


    /// <summary>
    /// Reload
    /// </summary>
    public async Task Reload(OfferModelDB? selectedOffer)
    {
        OfferCurrent = selectedOffer;
        if (table is not null)
            await table.ReloadServerData();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        OfferCurrent = Offer;
        await base.OnInitializedAsync();
    }
}