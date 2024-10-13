////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Обновить (или создать) сессию опроса/анкеты
/// </summary>
public class UpdateOrCreateSessionDocumentReceive(IConstructorService conService)
    : IResponseReceive<SessionOfDocumentDataModelDB?, SessionOfDocumentDataModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateOrCreateSessionDocumentReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB?>> ResponseHandleAction(SessionOfDocumentDataModelDB? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<SessionOfDocumentDataModelDB> res = await conService.UpdateOrCreateSessionDocument(payload);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}
