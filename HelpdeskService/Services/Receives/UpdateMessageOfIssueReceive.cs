////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Update Message Of Issue
/// </summary>
public class UpdateMessageOfIssueReceive
    : IResponseReceive<UpdateMessageRequestModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateMessageOfIssueHelpdeskReceive;

    public Task<TResponseModel<bool?>> ResponseHandleAction(UpdateMessageRequestModel? payload)
    {
        TResponseModel<bool?> res = new();

        return Task.FromResult(res);
    }
}