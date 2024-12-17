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
/// Удалить оффер
/// </summary>
public class OfferDeleteReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<OfferDeleteReceive> loggerRepo)
    : IResponseReceive<int?, bool?>
{
    /// <summary>
    /// Удалить оффер
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferDeleteCommerceReceive;

    /// <summary>
    /// Удалить оффер
    /// </summary>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<bool?> res = new() { Response = false };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        int lc = await context
            .OrdersDocuments
            .Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.OfferId == req))
            .CountAsync();

        string msg;
        if (lc != 0)
        {
            msg = $"Оффер не может быть удалён т.к. используется в заказах: {lc} шт.";
            res.AddError(msg);
            loggerRepo.LogError(msg);
            return res;
        }

        res.Response = await context.Offers.Where(x => x.Id == req).ExecuteDeleteAsync() > 0;

        if (res.Response == true)
        {
            msg = "Команда успешно выполнена";
            res.AddSuccess(msg);
            loggerRepo.LogInformation($"{msg}. Оффер #{req} удалён");
        }
        else
        {
            msg = $"Оффер #{req} отсутствует в БД. Возможно, он был удалён ранее";
            res.AddInfo(msg);
            loggerRepo.LogWarning($"{msg}. Оффер #{req} удалён");
        }

        return res;
    }
}