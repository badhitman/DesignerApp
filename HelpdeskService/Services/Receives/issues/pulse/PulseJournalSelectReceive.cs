////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// PulseJournalReceive - of context user
/// </summary>
public class PulseJournalSelectReceive(IHelpdeskService hdRepo)
    : IResponseReceive<TAuthRequestModel<TPaginationRequestModel<UserIssueModel>>?, TResponseModel<TPaginationResponseModel<PulseViewModel>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PulseJournalHelpdeskSelectReceive;

    /// <summary>
    /// PulseJournalReceive - of context user
    /// </summary>
    public async Task<TResponseModel<TPaginationResponseModel<PulseViewModel>>?> ResponseHandleAction(TAuthRequestModel<TPaginationRequestModel<UserIssueModel>>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.PulseJournalSelect(req);
    }
}