////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Получить схему документа
/// </summary>
public class GetDocumentSchemeReceive(IConstructorService conService)
    : IResponseReceive<int?, DocumentSchemeConstructorModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetDocumentSchemeReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB?>> ResponseHandleAction(int? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<DocumentSchemeConstructorModelDB> res = await conService.GetDocumentScheme(payload.Value);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}
