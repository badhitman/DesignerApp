////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// RowsForOrderDeleteReceive
/// </summary>
public class RowsForOrderDeleteReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<int[]?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RowsDeleteFromOrderCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool?> res = new() { Response = true };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        int[] orders_ids = await context
            .OrdersDocuments
            .Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id))
            .Select(x => x.Id)
            .ToArrayAsync();

        DateTime dtu = DateTime.UtcNow;

        await context.OrdersDocuments
                .Where(x => orders_ids.Any(y => y == x.Id))
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        await context.RowsOfOrdersDocuments.Where(x => req.Any(y => y == x.Id)).ExecuteDeleteAsync();
        res.AddSuccess("Команда удаления успешно выполнена");
        return res;
    }
}