////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Удалить связь [таба/вкладки схемы документа] с [формой] 
/// </summary>
public class DeleteTabDocumentSchemeJoinFormReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<int>?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteTabDocumentSchemeJoinFormReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(TAuthRequestModel<int>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.DeleteTabDocumentSchemeJoinForm(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}
