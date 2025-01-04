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
: IResponseReceive<TPaginationRequestModel<OffersSelectRequestModel>?, TPaginationResponseModel<OfferModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferModelDB>?>> ResponseHandleAction(TPaginationRequestModel<OffersSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OfferModelDB> q = from o in context.Offers
                                     join n in context.Nomenclatures.Where(x => x.ContextName == req.Payload.ContextName) on o.NomenclatureId equals n.Id
                                     select o;

        if (req.Payload.NomenclatureFilter is not null && req.Payload.NomenclatureFilter.Length != 0)
            q = q.Where(x => req.Payload.NomenclatureFilter.Any(y => y == x.NomenclatureId));

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate);

        IOrderedQueryable<OfferModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
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
                Response = [.. await oq.Skip(req.PageNum * req.PageSize).Take(req.PageSize).Include(x => x.Nomenclature).ToArrayAsync()]
            }
        };
    }
}