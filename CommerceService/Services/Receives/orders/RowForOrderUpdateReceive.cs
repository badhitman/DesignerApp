////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// RowForOrderUpdateReceive
/// </summary>
public class RowForOrderUpdateReceive(ICommerceService commRepo, ILogger<RowForOrderUpdateReceive> loggerRepo)
    : IResponseReceive<RowOfOrderDocumentModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RowForOrderUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(RowOfOrderDocumentModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = await commRepo.RowForOrderUpdate(req);
        return new() { Messages = res.Messages, Response = res.Response };
    }
}