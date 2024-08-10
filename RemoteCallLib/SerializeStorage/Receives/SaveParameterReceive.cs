////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Save parameter
/// </summary>
public class SaveParameterReceive
    : IResponseReceive<StorageCloudParameterPayloadModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SaveCloudParameterReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<int?>> ResponseHandleAction(StorageCloudParameterPayloadModel? payload)
    {
        TResponseModel<int?> res = new();

        return Task.FromResult(res);
    }
}