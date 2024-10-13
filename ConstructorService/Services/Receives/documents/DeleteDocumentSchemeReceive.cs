////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Удалить схему документа
/// </summary>
public class DeleteDocumentSchemeReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<int>?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteDocumentSchemeReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(TAuthRequestModel<int>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.DeleteDocumentScheme(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}
