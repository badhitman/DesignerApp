////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Обновить/создать таб/вкладку схемы документа
/// </summary>
public class CreateOrUpdateTabOfDocumentSchemeReceive(IConstructorService conService) : IResponseReceive<TAuthRequestModel<EntryDescriptionOwnedModel>?, TResponseModel<TabOfDocumentSchemeConstructorModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CreateOrUpdateTabOfDocumentSchemeReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>?> ResponseHandleAction(TAuthRequestModel<EntryDescriptionOwnedModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.CreateOrUpdateTabOfDocumentScheme(payload);
    }
}
