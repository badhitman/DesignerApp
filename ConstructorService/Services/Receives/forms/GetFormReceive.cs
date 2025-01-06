////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Получить форму
/// </summary>
public class GetFormReceive(IConstructorService conService)
   : IResponseReceive<int, TResponseModel<FormConstructorModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetFormReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>?> ResponseHandleAction(int payload)
    {
        return await conService.GetForm(payload);
    }
}