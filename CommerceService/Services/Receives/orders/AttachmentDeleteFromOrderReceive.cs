////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Transmission.Receives.commerce;

/// <summary>
/// AttachmentDeleteFromOrderReceive
/// </summary>
public class AttachmentDeleteFromOrderReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<AttachmentDeleteFromOrderReceive> loggerRepo, IMongoDatabase mongoFs)
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
        AttachmentForOrderModelDB? file_db = await context.AttachmentsForOrders.FirstOrDefaultAsync(x => x.Id == req);
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
        }

        return new()
        {
            Response = 0 < await context.AttachmentsForOrders.Where(x => x.Id == req).ExecuteDeleteAsync()
        };
    }
}