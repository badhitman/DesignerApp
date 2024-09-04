﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// RowForOrderUpdateReceive
/// </summary>
public class RowForOrderUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<RowOfOrderDocumentModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RowForOrderUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(RowOfOrderDocumentModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;

        await context.OrdersDocuments
                .Where(x => x.Id == req.OrderDocumentId)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        if (req.Id < 0)
        {
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Товар добавлен");
            res.Response = req.Id;
            return res;
        }

        res.Response = await context.RowsOfOrdersDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Quantity, req.Quantity)
            .SetProperty(p => p.OfferId, req.OfferId));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }
}