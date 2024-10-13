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
   : IResponseReceive<int?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CheckAndNormalizeSortIndexForElementsOfDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.CheckAndNormalizeSortIndexForElementsOfDirectory(payload.Value);
        return new()
        {
            Messages = res.Messages,
        };
    }
}