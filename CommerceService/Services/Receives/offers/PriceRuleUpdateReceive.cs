////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// PriceRuleUpdateReceive
/// </summary>
public class PriceRuleUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<PriceRuleForOfferModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PriceRuleUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(PriceRuleForOfferModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        req.Name = req.Name.Trim();
        if (req.QuantityRule <= 1)
        {
            res.AddError("Количество должно быть больше одного");
            return res;
        }
        if (await context.PricesRules.AnyAsync(x => x.Id != req.Id && x.OfferId == req.OfferId && x.QuantityRule == req.QuantityRule))
        {
            res.AddError("Правило с таким количеством уже существует");
            return res;
        }

        if (req.Id < 1)
        {
            req.CreatedAtUTC = DateTime.UtcNow;
            req.LastAtUpdatedUTC = DateTime.UtcNow;
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Создано новое правило ценообразования");
        }
        else
        {
            await context
                .PricesRules
                .Where(x => x.Id == req.Id)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.IsDisabled, req.IsDisabled)
                .SetProperty(p => p.Name, req.Name)
                .SetProperty(p => p.PriceRule, req.PriceRule)
                .SetProperty(p => p.QuantityRule, req.QuantityRule)
                .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow));

            res.AddSuccess("Правило ценообразования обновлено");
        }

        return res;
    }
}