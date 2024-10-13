////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Получить сессию
/// </summary>
public class GetSessionDocumentReceive(IConstructorService conService)
    : IResponseReceive<SessionGetModel?, SessionOfDocumentDataModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetSessionDocumentReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB?>> ResponseHandleAction(SessionGetModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<SessionOfDocumentDataModelDB> res = await conService.GetSessionDocument(payload);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}
