////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Найти порцию сессий по имени поля (с пагинацией)
/// </summary>
public class FindSessionsDocumentsByFormFieldNameReceive(IConstructorService conService)
    : IResponseReceive<FormFieldModel, TResponseModel<EntryDictModel[]>>
{

    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FindSessionsDocumentsByFormFieldNameReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDictModel[]>?> ResponseHandleAction(FormFieldModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.FindSessionsDocumentsByFormFieldName(payload);
    }
}
