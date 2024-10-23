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
public class AttachmentForOrderReceive(
    IDbContextFactory<CommerceContext> commerceDbFactory,
    IHelpdeskRemoteTransmissionService HelpdeskRepo,
    ILogger<AttachmentForOrderReceive> loggerRepo)
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

        OrderDocumentModelDB? order_db = await context
            .OrdersDocuments
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == req.OrderDocumentId);

        if (order_db is null)
        {
            res.AddError($"Заказ #{req.OrderDocumentId} не найден");
            return res;
        }

        AttachmentForOrderModelDB? file_db = order_db.Attachments!
            .FirstOrDefault(x => x.FilePoint == req.FilePoint);

        PulseRequestModel reqPulse;

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

            if (order_db.HelpdeskId.HasValue && order_db.HelpdeskId.Value > 0)
            {
                reqPulse = new()
                {
                    Payload = new()
                    {
                        Payload = new()
                        {
                            Description = $"Файл (внешний) '{file_db.Name}' {GlobalTools.SizeDataAsString(file_db.FileSize)} [{nameof(file_db.FilePoint)}:{file_db.FilePoint}] добавлен.",
                            IssueId = order_db.HelpdeskId.Value,
                            PulseType = PulseIssuesTypesEnum.Files,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME
                        },
                        SenderActionUserId = GlobalStaticConstants.Roles.System,
                    }
                };
                await HelpdeskRepo.PulsePush(reqPulse);
            }

            return res;
        }
        else
        {
            await context
                .AttachmentsForOrders
                .Where(x => x.FilePoint == req.FilePoint)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.FileSize, req.FileSize)
                .SetProperty(p => p.Name, req.FileName));
            if (order_db.HelpdeskId.HasValue && order_db.HelpdeskId.Value > 0)
            {
                reqPulse = new()
                {
                    Payload = new()
                    {
                        Payload = new()
                        {
                            Description = $"Файл (внешний) '{file_db.Name}' {GlobalTools.SizeDataAsString(file_db.FileSize)} [{nameof(file_db.FilePoint)}:{file_db.FilePoint}] перезаписан.",
                            IssueId = order_db.HelpdeskId.Value,
                            PulseType = PulseIssuesTypesEnum.Files,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME
                        },
                        SenderActionUserId = GlobalStaticConstants.Roles.System,
                    }
                };

                await HelpdeskRepo.PulsePush(reqPulse);
            }
        }

        res.AddWarning("Файл уже в системе");
        return res;
    }
}