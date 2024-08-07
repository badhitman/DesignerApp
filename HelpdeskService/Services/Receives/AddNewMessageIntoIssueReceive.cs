////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// AddNewMessageIntoIssue
/// </summary>
public class AddNewMessageIntoIssueReceive
    : IResponseReceive<IssueMessageHelpdeskBaseModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddNewMessageIntoIssueHelpdeskReceive;

    public Task<TResponseModel<int?>> ResponseHandleAction(IssueMessageHelpdeskBaseModel? payload)
    {
        TResponseModel<int?> res = new();

        return Task.FromResult(res);
    }
}