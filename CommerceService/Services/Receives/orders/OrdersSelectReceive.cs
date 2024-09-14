////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrdersSelectReceive
/// </summary>
public class OrdersSelectReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>>?, TPaginationResponseModel<OrderDocumentModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>?>> ResponseHandleAction(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrderDocumentModelDB> q = context
            .OrdersDocuments
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.SenderActionUserId))
            q = q.Where(x => x.AuthorIdentityUserId == req.Payload.SenderActionUserId);

        if (req.Payload.Payload.OrganizationFilter.HasValue && req.Payload.Payload.OrganizationFilter.Value != 0)
            q = q.Where(x => x.OrganizationId == req.Payload.Payload.OrganizationFilter);

        if (req.Payload.Payload.AddressForOrganizationFilter.HasValue && req.Payload.Payload.AddressForOrganizationFilter.Value != 0)
            q = q.Where(x => context.TabsAddressesForOrders.Any(y => y.OrderDocumentId == x.Id && y.AddressOrganizationId == req.Payload.Payload.AddressForOrganizationFilter));

        if (req.Payload.Payload.OfferFilter.HasValue && req.Payload.Payload.OfferFilter.Value != 0)
            q = q.Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.OfferId == req.Payload.Payload.OfferFilter));

        if (req.Payload.Payload.GoodsFilter.HasValue && req.Payload.Payload.GoodsFilter.Value != 0)
            q = q.Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.GoodsId == req.Payload.Payload.GoodsFilter));

        if (req.Payload.Payload.IsCartFilter == true)
            q = q.Where(x => x.HelpdeskId == null || x.HelpdeskId == 0);
        else if (req.Payload.Payload.IsCartFilter == false)
            q = q.Where(x => x.HelpdeskId != null && x.HelpdeskId > 0);

        if (req.Payload.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.Payload.AfterDateUpdate);

        IOrderedQueryable<OrderDocumentModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
           ? q.OrderBy(x => x.CreatedAtUTC)
           : q.OrderByDescending(x => x.CreatedAtUTC);

        IQueryable<OrderDocumentModelDB> pq = oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDocumentModelDB, GoodsModelDB?> inc_query = pq
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = req.Payload.Payload.IncludeExternalData ? [.. await inc_query.ToArrayAsync()] : [.. await q.ToArrayAsync()]
            },
        }; ;
    }
}