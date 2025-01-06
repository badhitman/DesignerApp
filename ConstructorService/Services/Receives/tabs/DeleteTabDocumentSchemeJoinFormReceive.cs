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
    : IResponseReceive<TAuthRequestModel<int>?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteTabDocumentSchemeJoinFormReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TAuthRequestModel<int>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.DeleteTabDocumentSchemeJoinForm(payload);
    }
}
