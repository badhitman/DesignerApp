////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// CheckAndNormalizeSortIndexForElementsOfDirectoryReceive
/// </summary>
public class CheckAndNormalizeSortIndexForElementsOfDirectoryReceive(IConstructorService conService)
   : IResponseReceive<int, ResponseBaseModel>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CheckAndNormalizeSortIndexForElementsOfDirectoryReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(int payload)
    {
        return await conService.CheckAndNormalizeSortIndexForElementsOfDirectory(payload);
    }
}