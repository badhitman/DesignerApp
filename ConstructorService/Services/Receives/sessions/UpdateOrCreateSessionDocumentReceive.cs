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
    : IResponseReceive<SessionOfDocumentDataModelDB, TResponseModel<SessionOfDocumentDataModelDB>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateOrCreateSessionDocumentReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>?> ResponseHandleAction(SessionOfDocumentDataModelDB? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.UpdateOrCreateSessionDocument(payload);
    }
}
