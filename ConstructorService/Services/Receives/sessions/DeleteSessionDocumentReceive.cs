////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Удалить сессию опроса/анкеты
/// </summary>
public class DeleteSessionDocumentReceive(IConstructorService conService)
    : IResponseReceive<int?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteSessionDocumentReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(int? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.DeleteSessionDocument(payload.Value);
        return new()
        {
            Messages = res.Messages,
        };
    }
}