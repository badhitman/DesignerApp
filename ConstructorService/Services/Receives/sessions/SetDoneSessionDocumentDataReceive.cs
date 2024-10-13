////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Отправить опрос на проверку (от клиента)
/// </summary>
public class SetDoneSessionDocumentDataReceive(IConstructorService conService)
    : IResponseReceive<string?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetDoneSessionDocumentDataReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(string? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        var res = await conService.SetDoneSessionDocumentData(payload);
        return new()
        {
            Messages = res.Messages
        };
    }
}
