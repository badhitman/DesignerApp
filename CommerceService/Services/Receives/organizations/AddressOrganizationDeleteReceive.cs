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
/// AddressOrganizationDeleteReceive
/// </summary>
public class AddressOrganizationDeleteReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<AddressOrganizationDeleteReceive> loggerRepo)
    : IResponseReceive<int?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddressOrganizationDeleteCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<bool?> res = new() { Response = false };

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        int count = await context
            .OrdersDocuments
            .CountAsync(x => context.TabsAddressesForOrders.Any(y => y.OrderDocumentId == x.Id && y.AddressOrganizationId == req));

        if (count != 0)
            res.AddError($"Адрес используется в заказах: {count} шт.");

        //count = await context
        //    .OrdersDocuments
        //    .CountAsync(x => context.TabsAddressesForOrders.Any(y => y.OrderDocumentId == x.Id && y.DeliveryAddressId == req));

        //if (count != 0)
        //    res.AddError($"Адрес указан в доставке: {count} шт.");

        if (!res.Success())
            return res;

        await context.AddressesOrganizations.Where(x => x.Id == req).ExecuteDeleteAsync();
        res.AddSuccess("Команда успешно выполнена");
        res.Response = true;
        return res;
    }
}