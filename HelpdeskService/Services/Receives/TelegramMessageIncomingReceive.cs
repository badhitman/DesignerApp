////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// TelegramMessageIncomingReceive
/// </summary>
public class TelegramMessageIncomingReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TelegramIncomingMessageModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IncomingTelegramMessageHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TelegramIncomingMessageModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.TelegramMessageIncoming(req);
    }
}