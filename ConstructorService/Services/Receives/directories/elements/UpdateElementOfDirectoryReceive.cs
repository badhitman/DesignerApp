////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Обновить элемент справочника
/// </summary>
public class UpdateElementOfDirectoryReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<EntryDescriptionModel>, ResponseBaseModel>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateElementOfDirectoryReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TAuthRequestModel<EntryDescriptionModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.UpdateElementOfDirectory(payload);
    }
}