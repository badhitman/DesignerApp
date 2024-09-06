////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// DeliveryOrderUpdateReceive
/// </summary>
public class DeliveryOrderUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<DeliveryForOrderUpdateRequestModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeliveryOrderUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(DeliveryForOrderUpdateRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        if (req.SetAction)
        {
            AddressForOrderModelDb? dlr = await context.AddressesForOrders
                .FirstOrDefaultAsync(x => x.AddressOrganizationId == req.DeliveryAddressId && x.OrderDocumentId == req.OrderDocumentId);

            if (dlr is null)
            {
                dlr = new()
                {
                    AddressOrganizationId = req.DeliveryAddressId,
                    OrderDocumentId = req.OrderDocumentId,
                    Status = HelpdeskIssueStepsEnum.Created,
                    DeliveryPrice = req.Price,
                };
                await context.AddAsync(dlr);
                await context.SaveChangesAsync();                
                res.AddSuccess("Адрес доставки добавлен к заказу");
            }
            else
            {
                await context.AddressesForOrders
                    .Where(x => x.AddressOrganizationId == req.DeliveryAddressId && x.OrderDocumentId == req.OrderDocumentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.DeliveryPrice, req.Price).SetProperty(p => p.Status, req.Status));
            }
            res.Response = dlr.Id;
        }
        else
        {
            res.Response = await context.AddressesForOrders
                .Where(x => x.AddressOrganizationId == req.DeliveryAddressId && x.OrderDocumentId == req.OrderDocumentId)
                .ExecuteDeleteAsync();

            if (res.Response > 0)
                res.AddSuccess("Адрес доставки удалён из заказа");
            else
                res.AddInfo("Адрес доставки отсутствует");
        }

        return res;
    }
}