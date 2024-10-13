////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Получить элементы справочника/списка
/// </summary>
public class GetElementsOfDirectoryReceive(IConstructorService conService)
    : IResponseReceive<int?, List<EntryModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetElementsOfDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<List<EntryModel>?>> ResponseHandleAction(int? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.GetElementsOfDirectory(payload.Value);
        return new()
        {
            Messages = res.Messages,
        };
    }
}