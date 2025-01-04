////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Сдвинуть связь [таба/вкладки схемы документа] с [формой] (изменение сортировки/последовательности)
/// </summary>
public class MoveTabDocumentSchemeJoinFormReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<MoveObjectModel>?, TabOfDocumentSchemeConstructorModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MoveTabDocumentSchemeJoinFormReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB?>> ResponseHandleAction(TAuthRequestModel<MoveObjectModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<TabOfDocumentSchemeConstructorModelDB> res = await conService.MoveTabDocumentSchemeJoinForm(payload);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}
