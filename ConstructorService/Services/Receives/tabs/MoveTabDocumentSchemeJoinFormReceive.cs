////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Сдвинуть связь [таба/вкладки схемы документа] с [формой] (изменение сортировки/последовательности)
/// </summary>
public class MoveTabDocumentSchemeJoinFormReceive(IConstructorService conService) : IResponseReceive<TAuthRequestModel<MoveObjectModel>?, TResponseModel<TabOfDocumentSchemeConstructorModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MoveTabDocumentSchemeJoinFormReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>?> ResponseHandleAction(TAuthRequestModel<MoveObjectModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.MoveTabDocumentSchemeJoinForm(payload);
    }
}
