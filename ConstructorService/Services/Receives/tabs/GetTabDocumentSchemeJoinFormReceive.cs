////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Получить связь [таба/вкладки схемы документа] с [формой]
/// </summary>
public class GetTabDocumentSchemeJoinFormReceive(IConstructorService conService)
    : IResponseReceive<int?, FormToTabJoinConstructorModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetTabDocumentSchemeJoinFormReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FormToTabJoinConstructorModelDB?>> ResponseHandleAction(int? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<FormToTabJoinConstructorModelDB> res = await conService.GetTabDocumentSchemeJoinForm(payload.Value);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}
