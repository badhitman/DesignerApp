////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// GoodsReadReceive
/// </summary>
public class GoodsReadReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<int[]?, GoodsModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GoodsReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<GoodsModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        return new()
        {
            Response = await context
            .Goods
            .Where(x => req.Any(y => x.Id == y))
            .Include(x => x.ProductsOffers)
            .ToArrayAsync()
        };
    }
}