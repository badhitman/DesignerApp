////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AddressOrganizationDeleteReceive
/// </summary>
public class AddressOrganizationDeleteReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<int?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddressOrganizationDeleteCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool?> res = new() { Response = false };

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        OrderDocumentModelDB[] links_deliveries = await context
            .OrdersDocuments
            .Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.AddressOrganizationId == req))
            .ToArrayAsync();

        OrderDocumentModelDB[] links_documents = await context
            .OrdersDocuments
            .Where(x => context.Deliveries.Any(y => y.OrderDocumentId == x.Id && y.AddressOrganizationId == req))
            .ToArrayAsync();

        if (links_deliveries.Length != 0)
            res.AddError($"Адрес указан в доставке: {links_deliveries.Length} шт.");

        if (links_documents.Length != 0)
            res.AddError($"Адрес используется в заказах: {links_documents.Length} шт.");

        if (!res.Success())
            return res;

        await context.AddressesOrganizations.Where(x => x.Id == req).ExecuteDeleteAsync();
        res.AddSuccess("Команда успешно выполнена");
        res.Response = true;
        return res;
    }
}