////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary> 
/// Обновить/создать схему документа
/// </summary>
public class UpdateOrCreateDocumentSchemeReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<EntryConstructedModel>?, DocumentSchemeConstructorModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateOrCreateDocumentSchemeReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB?>> ResponseHandleAction(TAuthRequestModel<EntryConstructedModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<DocumentSchemeConstructorModelDB> res = await conService.UpdateOrCreateDocumentScheme(payload);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}
