////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Прочитать рубрику (со всеми вышестоящими владельцами)
/// </summary>
public class RubricReadReceive(IHelpdeskService hdRepo)
    : IResponseReceive<int?, List<RubricIssueHelpdeskModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricForIssuesReadHelpdeskReceive;

    /// <summary>
    /// Прочитать рубрику (со всеми вышестоящими владельцами)
    /// </summary>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>?>> ResponseHandleAction(int? rubricId)
    {
        ArgumentNullException.ThrowIfNull(rubricId);
        TResponseModel<List<RubricIssueHelpdeskModelDB>?> res = await hdRepo.RubricRead(rubricId.Value);
        return res;
    }
}