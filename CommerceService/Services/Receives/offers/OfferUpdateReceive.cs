////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OfferUpdateReceive
/// </summary>
public class OfferUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<OfferGoodModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(OfferGoodModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;
        if (req.Id < 0)
        {
            OfferGoodModelDB offer_db = new()
            {
                Name = req.Name,
                IsDisabled = req.IsDisabled,
                Multiplicity = req.Multiplicity,
                GoodId = req.GoodId,
                OfferUnit = req.OfferUnit,
                Price = req.Price,
                LastAtUpdatedUTC = dtu,
            };

            await context.AddAsync(offer_db);
            await context.SaveChangesAsync();
            res.AddSuccess("Предложение добавлено");
            res.Response = offer_db.Id;
            return res;
        }

        res.Response = await context.OffersGoods
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.IsDisabled, req.IsDisabled)
            .SetProperty(p => p.Multiplicity, req.Multiplicity)
            .SetProperty(p => p.GoodId, req.GoodId)
            .SetProperty(p => p.OfferUnit, req.OfferUnit)
            .SetProperty(p => p.Price, req.Price)
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }
}