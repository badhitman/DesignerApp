////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// GoodsUpdateReceive
/// </summary>
public class GoodsUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<GoodsModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GoodsUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(GoodsModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;
        if (req.Id < 1)
        {
            if (await context.Goods.AnyAsync(x => x.Name == req.Name && x.BaseUnit == req.BaseUnit))
            {
                res.AddError("Такая номенклатура уже существует");
                return res;
            }

            GoodsModelDB goods_db = new()
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

        if (await context.Goods.AnyAsync(x => x.Id != req.Id && x.Name == req.Name && x.BaseUnit == req.BaseUnit))
        {
            res.AddError("Такая номенклатура уже существует. Измените название или единицу измерения.");
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