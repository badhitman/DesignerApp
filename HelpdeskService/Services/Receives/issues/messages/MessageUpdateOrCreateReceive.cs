////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Сообщение в обращение
/// </summary>
public class MessageUpdateOrCreateReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TAuthRequestModel<IssueMessageHelpdeskBaseModel>, TResponseModel<int>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessageOfIssueUpdateHelpdeskReceive;

    /// <summary>
    /// Сообщение в обращение
    /// </summary>
    public async Task<TResponseModel<int>?> ResponseHandleAction(TAuthRequestModel<IssueMessageHelpdeskBaseModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.MessageUpdateOrCreate(req);
    }
}