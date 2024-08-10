////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Read parameter
/// </summary>
public class ReadParameterReceive
    : IResponseReceive<StorageCloudParameterReadModel?, StorageCloudParameterPayloadModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadCloudParameterReceive;

    /// <inheritdoc/>
    public Task<TResponseModel<StorageCloudParameterPayloadModel?>> ResponseHandleAction(StorageCloudParameterReadModel? payload)
    {
        TResponseModel<StorageCloudParameterPayloadModel?> res = new();

        return Task.FromResult(res);
    }
}