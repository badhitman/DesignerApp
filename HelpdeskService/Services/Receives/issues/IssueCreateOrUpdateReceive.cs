////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Create (or update) Issue: Рубрика, тема и описание
/// </summary>
public class IssueCreateOrUpdateReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TAuthRequestModel<IssueUpdateRequestModel>?, int>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> ResponseHandleAction(TAuthRequestModel<IssueUpdateRequestModel>? issue_upd)
    {
        ArgumentNullException.ThrowIfNull(issue_upd);
        return await hdRepo.IssueCreateOrUpdate(issue_upd);
    }
}