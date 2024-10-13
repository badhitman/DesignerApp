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
    : IResponseReceive<SessionStatusModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetStatusSessionDocumentReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(SessionStatusModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.SetStatusSessionDocument(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}
