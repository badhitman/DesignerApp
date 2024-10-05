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
/// Прикрепить файл к заказу (счёт, акт и т.п.)
/// </summary>
public class AttachmentForOrderReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<AttachmentForOrderReceive> loggerRepo)
    : IResponseReceive<AttachmentForOrderRequestModel?, int?>
{
    /// <summary>
    /// Прикрепить файл к заказу (счёт, акт и т.п.)
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AttachmentAddToOrderCommerceReceive;

    /// <summary>
    /// Прикрепить файл к заказу (счёт, акт и т.п.)
    /// </summary>
    public async Task<TResponseModel<int?>> ResponseHandleAction(AttachmentForOrderRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        AttachmentForOrderModelDB? file_db = await context.AttachmentsForOrders.FirstOrDefaultAsync(x => x.FilePoint == req.FilePoint);

        if (file_db is null)
        {
            file_db = new()
            {
                FilePoint = req.FilePoint,
                FileSize = req.FileSize,
                Name = req.FileName,
                OrderDocumentId = req.OrderDocumentId,
                CreatedAtUTC = DateTime.UtcNow,
            };
            await context.AddAsync(file_db);
            await context.SaveChangesAsync();
            res.Response = file_db.Id;
            res.AddSuccess("Файл добавлен");
            return res;
        }
        else
            await context
                .AttachmentsForOrders
                .Where(x => x.FilePoint == req.FilePoint)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.FileSize, req.FileSize)
                .SetProperty(p => p.Name, req.FileName));

        res.AddWarning("Файл уже в системе");

        return res;
    }
}