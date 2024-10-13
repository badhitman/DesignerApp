////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Создать элемент справочника
/// </summary>
public class CreateElementForDirectoryReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<OwnedNameModel>?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CreateElementForDirectoryReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(TAuthRequestModel<OwnedNameModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.CreateElementForDirectory(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}