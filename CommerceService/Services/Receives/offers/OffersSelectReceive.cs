////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

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

        TResponseModel<TPaginationResponseModel<OfferGoodModelDB>?> res = new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
            }
        };

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OfferGoodModelDB> q = context
            .OffersGoods
            .AsQueryable();

        if(req.Payload.GoodFilter.HasValue && req.Payload.GoodFilter.Value > 0)
            q = q.Where(x => x.GoodsId == req.Payload.GoodFilter);

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate);

        res.Response.Response = [.. await q
            .OrderBy(x => x.LastAtUpdatedUTC)
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .Include(x => x.Goods)
            .ToArrayAsync()];

        return res;
    }
}