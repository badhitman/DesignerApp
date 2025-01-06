////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Запрос схем документов
/// </summary>
public class RequestDocumentsSchemesReceive(IConstructorService conService)
    : IResponseReceive<RequestDocumentsSchemesModel?, TPaginationResponseModel<DocumentSchemeConstructorModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RequestDocumentsSchemesReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<DocumentSchemeConstructorModelDB>?> ResponseHandleAction(RequestDocumentsSchemesModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.RequestDocumentsSchemes(payload);
    }
}