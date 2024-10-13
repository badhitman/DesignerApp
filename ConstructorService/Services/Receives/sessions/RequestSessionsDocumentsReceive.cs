////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Запросить порцию сессий (с пагинацией)
/// </summary>
public class RequestSessionsDocumentsReceive(IConstructorService conService)
    : IResponseReceive<RequestSessionsDocumentsRequestPaginationModel?, TPaginationResponseModel<SessionOfDocumentDataModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RequestSessionsDocumentsReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<SessionOfDocumentDataModelDB>?>> ResponseHandleAction(RequestSessionsDocumentsRequestPaginationModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return new()
        {
            Response = await conService.RequestSessionsDocuments(payload)
        };
    }
}
