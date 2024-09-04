////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrdersReadReceive
/// </summary>
public class OrdersReadReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<int[]?, OrderDocumentModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<OrderDocumentModelDB[]?> res = new();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        res.Response = await context
            .OrdersDocuments
            .Where(x => req.Any(y => x.Id == y))
            .Include(x => x.Attachments)
            .Include(x => x.Rows)
            .Include(x => x.Deliveries)
            .ToArrayAsync();

        return res;
    }
}