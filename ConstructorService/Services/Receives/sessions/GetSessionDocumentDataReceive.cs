////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Получить сессию
/// </summary>
public class GetSessionDocumentDataReceive(IConstructorService conService)
    : IResponseReceive<string, TResponseModel<SessionOfDocumentDataModelDB>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetSessionDocumentDataReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>?> ResponseHandleAction(string? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.GetSessionDocumentData(payload);
    }
}
