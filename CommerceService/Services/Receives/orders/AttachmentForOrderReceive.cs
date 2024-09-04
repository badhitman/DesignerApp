////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AttachmentForOrderReceive
/// </summary>
public class AttachmentForOrderReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<AttachmentForOrderRequestModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AttachmentAddToOrderCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(AttachmentForOrderRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
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

        res.AddWarning("Файл уже в системе");

        return res;
    }
}