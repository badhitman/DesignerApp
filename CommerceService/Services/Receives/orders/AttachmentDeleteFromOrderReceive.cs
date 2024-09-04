////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AttachmentDeleteFromOrderReceive
/// </summary>
public class AttachmentDeleteFromOrderReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<int?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AttachmentDeleteFromOrderCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        return new()
        {
            Response = 0 < await context.AttachmentsForOrders.Where(x => x.Id == req).ExecuteDeleteAsync()
        };
    }
}