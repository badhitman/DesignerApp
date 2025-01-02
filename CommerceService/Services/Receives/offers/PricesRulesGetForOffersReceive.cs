////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// PricesRulesGetForOffersReceive
/// </summary>
public class PricesRulesGetForOffersReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<int[], PriceRuleForOfferModelDB[]>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PricesRulesGetForOfferCommerceReceive;

    /// <inheritdoc/>
    public async Task<PriceRuleForOfferModelDB[]?> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<PriceRuleForOfferModelDB[]?> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        return await context
            .PricesRules.Where(x => req.Any(y => x.OfferId == y))
            .Include(x => x.Offer)
            .ToArrayAsync();
    }
}