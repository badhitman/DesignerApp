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
/// PaymentDocumentUpdateReceive
/// </summary>
public class PaymentDocumentUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<PaymentDocumentUpdateReceive> loggerRepo)
    : IResponseReceive<PaymentDocumentBaseModel?, TResponseModel<int>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PaymentDocumentUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>?> ResponseHandleAction(PaymentDocumentBaseModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = new() { Response = 0 };

        if (req.Amount <= 0)
        {
            res.AddError("Сумма платежа должна быть больше нуля");
            return res;
        }
        if (req.OrderDocumentId < 1)
        {
            res.AddError("Не указан документ-заказ");
            return res;
        }

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;

        PaymentDocumentModelDb? payment_db = null;
        if (!string.IsNullOrWhiteSpace(req.ExternalDocumentId))
        {
            payment_db = await context
               .PaymentsDocuments
               .FirstOrDefaultAsync(x => x.ExternalDocumentId == req.ExternalDocumentId);

            req.Id = req.Id > 0 ? req.Id : payment_db?.Id ?? 0;
        }

        if (req.Id < 1)
        {
            payment_db = new()
            {
                Name = req.Name,
                Amount = req.Amount,
                OrderDocumentId = req.OrderDocumentId,
                ExternalDocumentId = req.ExternalDocumentId,
            };

            await context.AddAsync(payment_db);

            await context.SaveChangesAsync();

            res.AddSuccess("Платёж добавлен");
            res.Response = req.Id;
            return res;
        }

        res.Response = await context.PaymentsDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.Amount, req.Amount));

        await context.OrdersDocuments
               .Where(x => x.Id == req.OrderDocumentId)
               .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));


        if (!string.IsNullOrWhiteSpace(req.ExternalDocumentId) && payment_db?.ExternalDocumentId != req.ExternalDocumentId)
            res.Response = await context.PaymentsDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set.SetProperty(p => p.ExternalDocumentId, req.ExternalDocumentId));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }
}