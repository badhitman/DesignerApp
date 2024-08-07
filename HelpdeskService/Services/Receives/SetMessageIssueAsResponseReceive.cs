////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// GetThemesIssues
/// </summary>
public class SetMessageIssueAsResponseReceive
    : IResponseReceive<SetMessageAsResponseIssueRequestModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetMessageAsResponseIssueHelpdeskReceive;

    public Task<TResponseModel<bool?>> ResponseHandleAction(SetMessageAsResponseIssueRequestModel? payload)
    {
        TResponseModel<bool?> res = new();

        return Task.FromResult(res);
    }
}