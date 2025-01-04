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
    : IResponseReceive<TAuthRequestModel<EntryDescriptionModel>?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateElementOfDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(TAuthRequestModel<EntryDescriptionModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.UpdateElementOfDirectory(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}