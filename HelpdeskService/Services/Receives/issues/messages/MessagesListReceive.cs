////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Получить сообщения для инцидента
/// </summary>
public class MessagesListReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TAuthRequestModel<int>?, TResponseModel<IssueMessageHelpdeskModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessagesOfIssueListHelpdeskReceive;

    /// <summary>
    /// Получить сообщения для инцидента
    /// </summary>
    public async Task<TResponseModel<IssueMessageHelpdeskModelDB[]>?> ResponseHandleAction(TAuthRequestModel<int>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.MessagesList(req);
    }
}