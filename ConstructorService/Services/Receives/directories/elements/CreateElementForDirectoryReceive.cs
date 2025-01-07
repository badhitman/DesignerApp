////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Создать элемент справочника
/// </summary>
public class CreateElementForDirectoryReceive(IConstructorService conService) : IResponseReceive<TAuthRequestModel<OwnedNameModel>?, TResponseModel<int>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CreateElementForDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>?> ResponseHandleAction(TAuthRequestModel<OwnedNameModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.CreateElementForDirectory(payload);
    }
}