////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OfferDeleteReceive
/// </summary>
public class OfferDeleteReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<int?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferDeleteCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool?> res = new() { Response = false };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        OrderDocumentModelDB[] links = await context
            .OrdersDocuments
            .Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.OfferId == req))
            .ToArrayAsync();

        if (links.Length != 0)
            res.AddError($"Предложение используется в заказах: {links.Length} шт.");

        if (!res.Success())
            return res;

        await context.OffersGoods.Where(x => x.Id == req).ExecuteDeleteAsync();
        res.AddSuccess("Команда успешно выполнена");
        res.Response = true;
        return res;
    }
}