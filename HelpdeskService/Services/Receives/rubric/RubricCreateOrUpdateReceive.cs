////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// CreateIssueTheme
/// </summary>
public class RubricCreateOrUpdateReceive(IHelpdeskService hdRepo, ILogger<RubricCreateOrUpdateReceive> loggerRepo)
    : IResponseReceive<RubricIssueHelpdeskModelDB?, TResponseModel<int>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricForIssuesUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>?> ResponseHandleAction(RubricIssueHelpdeskModelDB? rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(rubric)}");
        return await hdRepo.RubricCreateOrUpdate(rubric);
    }
}