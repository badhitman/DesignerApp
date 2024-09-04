﻿////////////////////////////////////////////////
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
: IResponseReceive<TPaginationRequestModel<OrdersSelectRequestModel>?, TPaginationResponseModel<OrderDocumentModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>?>> ResponseHandleAction(TPaginationRequestModel<OrdersSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 10)
            req.PageSize = 10;

        TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>?> res = new()
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

        IQueryable<OrderDocumentModelDB> q = context
            .OrdersDocuments
            .AsQueryable();

        if (req.Payload.CartOnly)
            q = q.Where(x => x.HelpdeskId == null || x.HelpdeskId == 0);

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate);

        res.Response.Response = [.. await q
            .OrderBy(x => x.LastAtUpdatedUTC)
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .ToArrayAsync()];

        return res;
    }
}