////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OffersReadReceive
/// </summary>
public class OffersReadReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<int[]?, OfferGoodModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OfferGoodModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<OfferGoodModelDB[]?> res = new();
        if (!req.Any(x => x > 0))
        {
            res.AddError("Пустой запрос");
            return res;
        }
        req = [.. req.Where(x => x > 0)];
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        return new TResponseModel<OfferGoodModelDB[]?>()
        {
            Response = await context.OffersGoods.Where(x => req.Any(y => x.Id == y)).ToArrayAsync()
        };
    }
}