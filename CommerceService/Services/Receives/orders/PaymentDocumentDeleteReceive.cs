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
/// PaymentDocumentDeleteReceive
/// </summary>
public class PaymentDocumentDeleteReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<PaymentDocumentDeleteReceive> loggerRepo)
    : IResponseReceive<int?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PaymentDocumentDeleteCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<bool?> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;
        await context.OrdersDocuments
                .Where(x => context.PaymentsDocuments.Any(y => y.Id == req && y.OrderDocumentId == x.Id))
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.Response = 0 < await context.PaymentsDocuments.Where(x => x.Id == req).ExecuteDeleteAsync();

        return res;
    }
}