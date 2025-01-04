////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Получить рубрики
/// </summary>
public class RubricsGetReceive(IHelpdeskService hdRepo)
    : IResponseReceive<int[]?, List<RubricIssueHelpdeskModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricsForIssuesGetHelpdeskReceive;

    /// <summary>
    /// Получить рубрики
    /// </summary>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>?>> ResponseHandleAction(int[]? rubricsIds)
    {
        ArgumentNullException.ThrowIfNull(rubricsIds);
        TResponseModel<List<RubricIssueHelpdeskModelDB>> res = await hdRepo.RubricsGet(rubricsIds);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}