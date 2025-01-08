////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// StatusOrderChangeByHelpdeskDocumentIdReceive
/// </summary>
public class StatusOrderChangeByHelpdeskDocumentIdReceive(ICommerceService commRepo, ILogger<StatusOrderChangeByHelpdeskDocumentIdReceive> LoggerRepo) : IResponseReceive<StatusChangeRequestModel?, TResponseModel<bool>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.StatusChangeOrderByHelpDeskDocumentIdReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>?> ResponseHandleAction(StatusChangeRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        return await commRepo.StatusesOrdersChangeByHelpdeskDocumentId(req);
    }
}