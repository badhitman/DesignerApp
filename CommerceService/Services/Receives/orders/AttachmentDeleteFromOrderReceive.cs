////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using MongoDB.Driver;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AttachmentDeleteFromOrderReceive
/// </summary>
public class AttachmentDeleteFromOrderReceive(
    IHelpdeskRemoteTransmissionService HelpdeskRepo,
    IDbContextFactory<CommerceContext> commerceDbFactory,
    ILogger<AttachmentDeleteFromOrderReceive> loggerRepo,
    IMongoDatabase mongoFs)
    : IResponseReceive<int?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AttachmentDeleteFromOrderCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        AttachmentForOrderModelDB? file_db = await context
            .AttachmentsForOrders
            .Include(x => x.OrderDocument)
            .FirstOrDefaultAsync(x => x.Id == req);

        if (file_db is not null && !string.IsNullOrWhiteSpace(file_db.FilePoint))
        {
            try
            {
                IGridFSBucket gridFS = new GridFSBucket(mongoFs);
                await gridFS.DeleteAsync(new MongoDB.Bson.ObjectId(file_db.FilePoint));
            }
            catch (Exception ex)
            {
                loggerRepo.LogError(ex, JsonConvert.SerializeObject(file_db));
            }

            if (file_db.OrderDocument!.HelpdeskId.HasValue && file_db.OrderDocument.HelpdeskId.Value > 0)
            {
                PulseRequestModel reqPulse = new()
                {
                    Payload = new()
                    {
                        Payload = new()
                        {
                            Description = $"Файл (внешний) '{file_db.Name}' {GlobalTools.SizeDataAsString(file_db.FileSize)} [{nameof(file_db.FilePoint)}:{file_db.FilePoint}] удалён.",
                            IssueId = file_db.OrderDocument.HelpdeskId.Value,
                            PulseType = PulseIssuesTypesEnum.Files,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME
                        },
                        SenderActionUserId = GlobalStaticConstants.Roles.System,
                    }
                };

                await HelpdeskRepo.PulsePush(reqPulse);
            }
        }

        return new()
        {
            Response = 0 < await context.AttachmentsForOrders.Where(x => x.Id == req).ExecuteDeleteAsync()
        };
    }
}