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
    MudTable<OrganizationModelDB>? table;

    async Task SetContract(OrganizationModelDB org)
    {
        if (IsBusyProgress || CurrentUserSession is null)
            return;

        TAuthRequestModel<OrganizationOfferToggleModel> req = new()
        {
            SenderActionUserId = CurrentUserSession.UserId,
            Payload = new()
            {
                OrganizationId = org.Id,
                OfferId = Offer?.Id,
            }
        };
        await SetBusy();

        if (Offer is null || Offer.Id < 1)
            req.Payload.SetValue = org.Contractors?.Any(x => x.OrganizationId == org.Id && (x.OfferId == null || x.OfferId < 1)) != true;
        else
            req.Payload.SetValue = org.Contractors?.Any(x => x.OrganizationId == org.Id && x.OfferId.HasValue && x.OfferId == Offer.Id) != true;

        TResponseModel<bool> res = await CommerceRepo.OrganizationOfferContractUpdate(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await SetBusy(false);
        if (table is not null)
            await table.ReloadServerData();
    }

    string GetContractCss(OrganizationModelDB org)
    {
        string css;

        if (Offer is null || Offer.Id < 1)
            css = org.Contractors?
                .Any(x => x.OfferId == null && x.OrganizationId == org.Id) == true ? "success" : "secondary";
        else
            css = org.Contractors?
                .Any(x => x.OfferId == Offer.Id && x.OrganizationId == org.Id) == true ? "success" : "secondary";

        return css;
    }

    string GetContractTitle(OrganizationModelDB org)
    {
        string title;

        if (Offer is null || Offer.Id < 1)
            title = org.Contractors?
                .Any(x => x.OfferId == null && x.OrganizationId == org.Id) == true ? "глобальное выдано. отозвать?" : "глобального нет. предоставить?";
        else
            title = org.Contractors?
                .Any(x => x.OfferId == Offer.Id && x.OrganizationId == org.Id) == true ? "локальное выдано. отозвать?" : "локального нет. предоставить?";

        return title;
    }

    string GetContractInfo(OrganizationModelDB org)
    {
        string title;
        IQueryable<OrganizationContractorModel>? q;
        if (Offer is null || Offer.Id < 1)
        {
            q = org.Contractors?
                .Where(x => x.OfferId != null && x.OrganizationId == org.Id)
                .AsQueryable();

            title = q?.Any() == true ? $"(+локальные: {string.Join("; ", q.Select(x => x.Offer!.Name))};)" : "";
        }
        else
        {
            q = org.Contractors?
                .Where(x => x.OrganizationId == org.Id)
                .AsQueryable();

            title = q?
                .Any(x => x.OfferId == null) == true ? "(+глобально!" : "";

            if (q?.Any(x => x.OfferId.HasValue && x.OfferId != Offer.Id) == true)
            {
                string _pr = string.Join(";", q.Where(x => x.OfferId.HasValue && x.OfferId != Offer.Id).Select(x => x.Offer!.Name));
                if (string.IsNullOrEmpty(title))
                    title = $"(ещё локальные:{_pr})";
                else
                    title += $" и локальные:{_pr})";
            }
            else if (!string.IsNullOrEmpty(title))
                title += ")";
        }

        return title;
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    async Task<TableData<OrganizationModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        if (CurrentUserSession is null)
            return new TableData<OrganizationModelDB>() { TotalItems = 0, Items = [] };

        TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req = new()
        {
            Payload = new()
            {
                IncludeExternalData = true,
                OffersFilter = OfferCurrent is null ? null : [OfferCurrent.Id],
            },
            SenderActionUserId = CurrentUserSession.UserId,
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        await SetBusy(token: token);
        TPaginationResponseModel<OrganizationModelDB> res = await CommerceRepo.OrganizationsSelect(req);
        await SetBusy(false, token: token);

        if ( res.Response is null)
            return new TableData<OrganizationModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<OrganizationModelDB>() { TotalItems = res.TotalRowsCount, Items = res.Response };
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