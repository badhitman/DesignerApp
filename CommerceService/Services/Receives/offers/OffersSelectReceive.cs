////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using System.Linq;

namespace Transmission.Receives.commerce;

/// <summary>
/// OffersSelectReceive
/// </summary>
public class OffersSelectReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<TPaginationRequestModel<OffersSelectRequestModel>?, TPaginationResponseModel<OfferGoodModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferGoodModelDB>?>> ResponseHandleAction(TPaginationRequestModel<OffersSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OfferGoodModelDB> q = context
            .OffersGoods
            .AsQueryable();

        if (req.Payload.GoodsFilter is not null && req.Payload.GoodsFilter.Length != 0)
            q = q.Where(x => req.Payload.GoodsFilter.Any(y => y == x.GoodsId));

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate);

        
         IOrderedQueryable<OfferGoodModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
           ? q.OrderBy(x => x.CreatedAtUTC)
           : q.OrderByDescending(x => x.CreatedAtUTC);
         
        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = [.. await oq.Skip(req.PageNum * req.PageSize).Take(req.PageSize).Include(x => x.Goods).ToArrayAsync()]
            }
        };
    }
}