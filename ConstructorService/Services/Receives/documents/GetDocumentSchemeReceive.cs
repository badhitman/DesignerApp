////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Получить схему документа
/// </summary>
public class GetDocumentSchemeReceive(IConstructorService conService) : IResponseReceive<int, TResponseModel<DocumentSchemeConstructorModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetDocumentSchemeReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>?> ResponseHandleAction(int payload)
    {
        return await conService.GetDocumentScheme(payload);
    }
}
