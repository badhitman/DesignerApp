////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using System.Globalization;

namespace Transmission.Receives.commerce;

/// <summary>
/// StatusChangeReceive
/// </summary>
public class StatusChangeReceive(
    IDbContextFactory<CommerceLayerContext> commerceDbFactory,
    ILogger<StatusChangeReceive> LoggerRepo)
    : IResponseReceive<StatusChangeRequestModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.StatusChangeOrderReceive;
    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(StatusChangeRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");

        using CommerceLayerContext context = await commerceDbFactory.CreateDbContextAsync();
        TResponseModel<bool?> res = new()
        {
            Response = await context
                    .OrdersDocuments
                    .Where(x => x.HelpdeskId == req.IssueId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.StatusDocument, req.Step)) != 0,
        };

        return res;
    }
}