////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Обновление номенклатуры
/// </summary>
public class GoodsUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<GoodsUpdateReceive> loggerRepo)
    : IResponseReceive<GoodsModelDB?, int?>
{
    /// <summary>
    /// Обновление номенклатуры
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GoodsUpdateCommerceReceive;

    /// <summary>
    /// Обновление номенклатуры
    /// </summary>
    public async Task<TResponseModel<int?>> ResponseHandleAction(GoodsModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        req.Name = req.Name.Trim();
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        string msg, about = $"'{req.Name}' /{req.BaseUnit}";
        GoodsModelDB? goods_db = await context.Goods.FirstOrDefaultAsync(x => x.Name == req.Name && x.BaseUnit == req.BaseUnit && x.Id != req.Id);
        if (goods_db is not null)
        {
            msg = $"Ошибка создания Номенклатуры {about}. Такой объект уже существует #{goods_db.Id}. Требуется уникальное сочетание имени и единицы измерения";
            loggerRepo.LogWarning(msg);
            res.AddError(msg);
            return res;
        }
        DateTime dtu = DateTime.UtcNow;

        if (req.Id < 1)
        {
            goods_db = new()
            {
                Name = req.Name,
                Description = req.Description,
                BaseUnit = req.BaseUnit,
                IsDisabled = req.IsDisabled,
                LastAtUpdatedUTC = dtu,
            };

            await context.AddAsync(goods_db);
            await context.SaveChangesAsync();
            msg = $"Номенклатура {about} создана #{goods_db.Id}";
            loggerRepo.LogInformation(msg);
            res.AddSuccess(msg);
            res.Response = goods_db.Id;
            return res;
        }

        res.Response = await context.Goods
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.Description, req.Description)
            .SetProperty(p => p.BaseUnit, req.BaseUnit)
            .SetProperty(p => p.IsDisabled, req.IsDisabled)
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        msg = $"Обновление номенклатуры {about} выполнено";
        loggerRepo.LogInformation(msg);
        res.AddSuccess(msg);
        return res;
    }
}