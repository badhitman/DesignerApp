﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using Newtonsoft.Json;

namespace Transmission.Receives.commerce;

/// <summary>
/// RowsForOrderDeleteReceive
/// </summary>
public class RowsForOrderDeleteReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<RowsForOrderDeleteReceive> loggerRepo)
    : IResponseReceive<int[]?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RowsDeleteFromOrderCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<bool?> res = new() { Response = true };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        int[] orders_ids = await context
            .OrdersDocuments
            .Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id))
            .Select(x => x.Id)
            .ToArrayAsync();

        if (orders_ids.Length == 0)
        {
            res.AddError($"Документы не найдены");
            return res;
        }

        DateTime dtu = DateTime.UtcNow;

        await context.OrdersDocuments
                .Where(x => orders_ids.Any(y => y == x.Id))
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        await context.RowsOfOrdersDocuments.Where(x => req.Any(y => y == x.Id)).ExecuteDeleteAsync();
        res.AddSuccess("Команда удаления успешно выполнена");
        return res;
    }
}