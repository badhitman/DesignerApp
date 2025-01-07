////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Регистрация события из обращения (логи).
/// </summary>
/// <remarks>
/// Плюс рассылка уведомлений участникам события.
/// </remarks>
public class PulseIssueReceive(IHelpdeskService hdRepo) : IResponseReceive<PulseRequestModel?, TResponseModel<bool>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PulseIssuePushHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>?> ResponseHandleAction(PulseRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.PulsePush(req);
    }
}