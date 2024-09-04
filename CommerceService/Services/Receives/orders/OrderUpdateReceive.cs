////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrderUpdateReceive
/// </summary>
public class OrderUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<OrderDocumentModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrderUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(OrderDocumentModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;
        if (req.Id < 0)
        {
            OrderDocumentModelDB order_db = new()
            {
                Name = req.Name,
                AuthorIdentityUserId = req.AuthorIdentityUserId,
                IsDisabled = req.IsDisabled,
                LastAtUpdatedUTC = dtu,
            };

            await context.AddAsync(order_db);
            await context.SaveChangesAsync();
            res.AddSuccess("Товар добавлен");
            res.Response = order_db.Id;
            return res;
        }

        res.Response = await context.OrdersDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.IsDisabled, req.IsDisabled)
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }
}