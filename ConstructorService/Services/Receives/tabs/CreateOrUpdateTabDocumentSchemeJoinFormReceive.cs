////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Обновить/создать связь [таба/вкладки схемы документа] с [формой]
/// </summary>
public class CreateOrUpdateTabDocumentSchemeJoinFormReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<FormToTabJoinConstructorModelDB>?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CreateOrUpdateTabDocumentSchemeJoinFormReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(TAuthRequestModel<FormToTabJoinConstructorModelDB>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.CreateOrUpdateTabDocumentSchemeJoinForm(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}
