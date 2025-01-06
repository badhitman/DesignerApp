////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Установить статус сессии (от менеджера)
/// </summary>
public class SetStatusSessionDocumentReceive(IConstructorService conService)
    : IResponseReceive<SessionStatusModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetStatusSessionDocumentReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(SessionStatusModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.SetStatusSessionDocument(payload);
    }
}
