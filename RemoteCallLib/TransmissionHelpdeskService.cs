////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

public class TransmissionHelpdeskService(IRabbitClient rabbitClient) : IHelpdeskRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> RubricForIssuesCreateOrUpdate(RubricIssueHelpdeskModelDB issueTheme)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.RubricForIssuesUpdateHelpdeskReceive, issueTheme);

    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>?>> RubricsForIssuesList(ProjectOwnedRequestModel req)
        => await rabbitClient.MqRemoteCall<List<RubricIssueHelpdeskModelDB>>(GlobalStaticConstants.TransmissionQueues.RubricsForIssuesListHelpdeskReceive, req);


    /// <inheritdoc/>
    public async Task<TResponseModel<int>> IssueCreateOrUpdate(IssueHelpdeskModelDB issue)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.IssueUpdateHelpdeskReceive, issue);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]?>> IssuesSelect(GetIssuesForUserRequestModel req)
        => await rabbitClient.MqRemoteCall<IssueHelpdeskModelDB[]>(GlobalStaticConstants.TransmissionQueues.IssuesSelectHelpdeskReceive, req);


    /// <inheritdoc/>
    public async Task<TResponseModel<int>> MessageForIssueCreateOrUpdate(IssueMessageHelpdeskBaseModel req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueUpdateHelpdeskReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> MessageOfIssueSetAsResponse(SetMessageAsResponseIssueRequestModel req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.MessageOfIssueSetAsResponseHelpdeskReceive, req);
}