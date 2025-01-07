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
public class OffersReadReceive(IDbContextFactory<CommerceContext> commerceDbFactory) : IResponseReceive<int[]?, TResponseModel<OfferModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OfferModelDB[]>?> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<OfferModelDB[]> res = new();
        if (!req.Any(x => x > 0))
        {
            res.AddError("Пустой запрос");
            return res;
        }
        req = [.. req.Where(x => x > 0)];
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        res.Response = await context.Offers.Where(x => req.Any(y => x.Id == y)).ToArrayAsync();
        return res;
    }
}