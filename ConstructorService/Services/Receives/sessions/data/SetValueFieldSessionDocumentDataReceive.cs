////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Установить значение свойства сессии
/// </summary>
public class SetValueFieldSessionDocumentDataReceive(IConstructorService conService)
    : IResponseReceive<SetValueFieldDocumentDataModel?, SessionOfDocumentDataModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetValueFieldSessionDocumentDataReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB?>> ResponseHandleAction(SetValueFieldDocumentDataModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<SessionOfDocumentDataModelDB> res = await conService.SetValueFieldSessionDocumentData(payload);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages
        };
    }
}
