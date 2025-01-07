////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Получить элемент справочника/перечисления/списка
/// </summary>
public class GetElementOfDirectoryReceive(IConstructorService conService) : IResponseReceive<int, TResponseModel<EntryDescriptionModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetElementOfDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDescriptionModel>?> ResponseHandleAction(int payload)
    {
        return await conService.GetElementOfDirectory(payload);
    }
}