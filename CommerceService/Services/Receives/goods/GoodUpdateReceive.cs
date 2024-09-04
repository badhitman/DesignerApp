////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// GoodUpdateReceive
/// </summary>
public class GoodUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<GoodModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GoodsUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(GoodModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;
        if (req.Id < 0)
        {
            GoodModelDB goods_db = new()
            {
                Name = req.Name,
                BaseUnit = req.BaseUnit,
                IsDisabled = req.IsDisabled,
                LastAtUpdatedUTC = dtu,
            };

            await context.AddAsync(goods_db);
            await context.SaveChangesAsync();
            res.AddSuccess("Товар добавлен");
            res.Response = goods_db.Id;
            return res;
        }

        res.Response = await context.Goods
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.BaseUnit, req.BaseUnit)
            .SetProperty(p => p.IsDisabled, req.IsDisabled)
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }
}